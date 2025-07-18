/*
 * Copyright (c) 2016 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualBasic;
using Native = Interop.ScreenMirroring;

namespace Tizen.Multimedia.Remoting
{
    /// <summary>
    /// Provides the ability to connect to and disconnect from a screen mirroring source,
    /// start, pause, and resume the screen mirroring as a sink.
    /// </summary>
    /// <since_tizen> 4 </since_tizen>
    public class ScreenMirroring : IDisposable, IDisplayable<ScreenMirroringErrorCode>
    {
        private const string Feature = "http://tizen.org/feature/network.wifi.direct.display";

        private const uint _port = 2022;
        private const uint _portMax = 65535;

        private IntPtr _handle;

        private AtomicState _state;

        private bool _disposed = false;

        private Native.StateChangedCallback _stateChangedCallback;
        private Native.SrcDisplayOrientationReceivedCallback _displayOrientationReceivedCallback;
        private Native.UibcInfoReceivedCallback _uibcInfoReceivedCallback;

        internal IntPtr Handle
        {
            get
            {
                ThrowIfDisposed();

                return _handle;
            }
        }

        private static bool IsSupported()
        {
            return System.Information.TryGetValue(Feature, out bool isSupported) ? isSupported : false;
        }

        /// <summary>
        /// Initializes a new instance of the ScreenMirroring class.
        /// </summary>
        /// <feature>http://tizen.org/feature/network.wifi.direct.display</feature>
        /// <exception cref="NotSupportedException">The feature is not supported.</exception>
        /// <since_tizen> 4 </since_tizen>
        public ScreenMirroring()
        {
            if (IsSupported() == false)
            {
                throw new PlatformNotSupportedException(
                    $"The feature({Feature}) is not supported on the current device.");
            }

            Native.Create(out _handle).ThrowIfError("Failed to create ScreenMirroring.");

            _state = new AtomicState();

            AudioInfo = new ScreenMirroringAudioInfo(this);
            VideoInfo = new ScreenMirroringVideoInfo(this);

            RegisterStateChangedEvent();
            RegisterDisplayOrientationChangedEvent();
            RegisterUibcInfoReceivedEvent();
        }

        /// <summary>
        /// Finalizes an instance of the ScreenMirroring class.
        /// </summary>
        ~ScreenMirroring()
        {
            Dispose(false);
        }

        /// <summary>
        /// Occurs when the state is changed.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public event EventHandler<ScreenMirroringStateChangedEventArgs> StateChanged;

        /// <summary>
        /// Occurs when an error occurs.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public event EventHandler<ScreenMirroringErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>
        /// Occurs when the display orientation of source is changed.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<ScreenMirroringDisplayOrientationChangedEventArgs> DisplayOrientationChanged;

        /// <summary>
        /// Occurs when the UIBC information is received from source.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler<ScreenMirroringUibcInfoReceivedEventArgs> UibcInfoReceived;

        #region Display support

        private Display _display;

        private void DetachDisplay()
        {
            if (_display != null)
            {
                _display.SetOwner(null);
                _display = null;
            }
        }

        private void SetDisplay(Display display)
        {
            if (display == null)
            {
                throw new ArgumentNullException(nameof(Display));
            }

            display.SetOwner(this);
            display.ApplyTo(this).ThrowIfError("Failed to set display.");

            _display = display;
        }

        ScreenMirroringErrorCode IDisplayable<ScreenMirroringErrorCode>.ApplyEvasDisplay(DisplayType type,
            ElmSharp.EvasObject evasObject)
        {
            Debug.Assert(Enum.IsDefined(typeof(DisplayType), type));

            return Native.SetDisplay(Handle, (int)type, evasObject);
        }

        ScreenMirroringErrorCode IDisplayable<ScreenMirroringErrorCode>.ApplyEcoreWindow(IntPtr windowHandle, Rectangle rect, Rotation rotation)
        {
            return Native.SetEcoreDisplay(Handle, windowHandle);
        }
        #endregion

        /// <summary>
        /// Gets the negotiated audio info.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public ScreenMirroringAudioInfo AudioInfo { get; }

        /// <summary>
        /// Gets the negotiated video info.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        public ScreenMirroringVideoInfo VideoInfo { get; }

        private bool IsConnected
        {
            get
            {
                return _state.IsOneOf(ScreenMirroringState.Connected, ScreenMirroringState.Playing,
                    ScreenMirroringState.Paused);
            }
        }

        internal void ThrowIfNotConnected()
        {
            ThrowIfDisposed();

            if (IsConnected == false)
            {
                throw new InvalidOperationException("ScreenMirroring is not connected.");
            }
        }

        /// <summary>
        /// Prepares the screen mirroring with the specified display.
        /// </summary>
        /// <remarks>
        /// The state must be <see cref="ScreenMirroringState.Idle"/>.<br/>
        /// <br/>
        /// All supported resolutions will be candidates.
        /// </remarks>
        /// <param name="display">The display where the mirroring will be played on.</param>
        /// <exception cref="ArgumentException">
        ///    <paramref name="display"/> has already been assigned to another.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="display"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The current state is not in the valid.<br/>
        ///     -or-<br/>
        ///     An internal error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <since_tizen> 4 </since_tizen>
        public void Prepare(Display display)
        {
            PrepareCore(display, (ScreenMirroringResolutions)0);
        }

        /// <summary>
        /// Prepares the screen mirroring with the specified display and resolutions.
        /// </summary>
        /// <remarks>
        /// The state must be <see cref="ScreenMirroringState.Idle"/>.
        /// </remarks>
        /// <param name="display">The display where the mirroring will be played on.</param>
        /// <param name="resolutions">The desired resolutions.</param>
        /// <exception cref="ArgumentException">
        ///    <paramref name="resolutions"/> contain invalid flags.<br/>
        ///    -or-<br/>
        ///    <paramref name="display"/> has already been assigned to another.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="display"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The current state is not in the valid.<br/>
        ///     -or-<br/>
        ///     An internal error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <since_tizen> 4 </since_tizen>
        public void Prepare(Display display, ScreenMirroringResolutions resolutions)
        {
            ValidationUtil.ValidateFlagsEnum(resolutions, (ScreenMirroringResolutions)((1 << 7) - 1), nameof(resolutions));

            PrepareCore(display, resolutions);
        }

        private void PrepareCore(Display display, ScreenMirroringResolutions resolutions)
        {
            ValidateState(ScreenMirroringState.Idle);

            Native.SetResolution(Handle, resolutions).ThrowIfError("Failed to set resolutions.");

            try
            {
                SetDisplay(display);

                Native.Prepare(Handle).ThrowIfError("Failed to prepare.");
            }
            catch
            {
                DetachDisplay();
                throw;
            }
        }

        private Task RunAsync(Func<IntPtr, ScreenMirroringErrorCode> func, string failMessage)
        {
            var tcs = new TaskCompletionSource<bool>();

            Task.Run(() =>
            {
                var ret = func(Handle);

                if (ret == ScreenMirroringErrorCode.None)
                {
                    tcs.SetResult(true);
                }
                else
                {
                    tcs.SetException(ret.AsException(failMessage));
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Creates the connection and ready for receiving data from a mirroring source.
        /// </summary>
        /// <param name="sourceIp">The source ip address to connect.</param>
        /// <remarks>
        /// The state must be <see cref="ScreenMirroringState.Prepared"/> state by
        /// <see cref="Prepare(Display, ScreenMirroringResolutions)"/>.<br/>
        /// The default port number is 2022.<br/>
        /// If you want to connect using different port number, please use <see cref="ConnectAsync(string, uint)"/>.
        /// </remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <privilege>http://tizen.org/privilege/internet</privilege>
        /// <exception cref="ArgumentException"><paramref name="sourceIp"/> is a zero-length string, contains only white space.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="sourceIp"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The current state is not in the valid.<br/>
        ///     -or-<br/>
        ///     An internal error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <exception cref="UnauthorizedAccessException">Caller does not have required permission.</exception>
        /// <seealso cref="ConnectAsync(string, uint)"/>
        /// <since_tizen> 4 </since_tizen>
        public Task ConnectAsync(string sourceIp)
        {
            return ConnectAsync(sourceIp, _port);
        }

        /// <summary>
        /// Creates the connection and ready for receiving data from a mirroring source with the given <paramref name="port"/>.
        /// </summary>
        /// <param name="sourceIp">The source ip address to connect.</param>
        /// <param name="port">The port number to connect. The max value is 65535.</param>
        /// <remarks>
        /// The state must be <see cref="ScreenMirroringState.Prepared"/> state by
        /// <see cref="Prepare(Display, ScreenMirroringResolutions)"/>.
        /// </remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <privilege>http://tizen.org/privilege/internet</privilege>
        /// <exception cref="ArgumentException"><paramref name="sourceIp"/> is a zero-length string, contains only white space.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="sourceIp"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="port"/> is greater than port max value(65535).</exception>
        /// <exception cref="InvalidOperationException">
        ///     The current state is not in the valid.<br/>
        ///     -or-<br/>
        ///     An internal error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <exception cref="UnauthorizedAccessException">Caller does not have required permission.</exception>
        /// <since_tizen> 9 </since_tizen>
        public Task ConnectAsync(string sourceIp, uint port)
        {
            if (sourceIp == null)
            {
                throw new ArgumentNullException(nameof(sourceIp));
            }
            if (string.IsNullOrWhiteSpace(sourceIp))
            {
                throw new ArgumentException($"{nameof(sourceIp)} is a zero-length string.", nameof(sourceIp));
            }
            if (port > _portMax)
            {
                throw new ArgumentOutOfRangeException(nameof(port), $"{nameof(port)} is greater than max port value(65535).");
            }

            ValidateState(ScreenMirroringState.Prepared);

            Native.SetIpAndPort(Handle, sourceIp, port.ToString()).ThrowIfError("Failed to set ip.");

            return RunAsync(Native.Connect, "Failed to connect.");
        }

        /// <summary>
        /// Starts mirroring from the source.
        /// </summary>
        /// <remarks>
        /// The state must be <see cref="ScreenMirroringState.Connected"/> state by
        /// <see cref="ConnectAsync(string)"/>.
        /// </remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <privilege>http://tizen.org/privilege/internet</privilege>
        /// <exception cref="InvalidOperationException">
        ///     The current state is not in the valid.<br/>
        ///     -or-<br/>
        ///     An internal error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <exception cref="UnauthorizedAccessException">Caller does not have required permission.</exception>
        /// <since_tizen> 4 </since_tizen>
        public Task StartAsync()
        {
            ValidateState(ScreenMirroringState.Connected);

            return RunAsync(Native.Start, "Failed to start.");
        }

        /// <summary>
        /// Pauses mirroring from the source.
        /// </summary>
        /// <remarks>
        /// The state must be <see cref="ScreenMirroringState.Playing"/> state by
        /// <see cref="StartAsync"/>.
        /// </remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <privilege>http://tizen.org/privilege/internet</privilege>
        /// <exception cref="InvalidOperationException">
        ///     The current state is not in the valid.<br/>
        ///     -or-<br/>
        ///     An internal error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <exception cref="UnauthorizedAccessException">Caller does not have required permission.</exception>
        /// <since_tizen> 4 </since_tizen>
        public Task PauseAsync()
        {
            ValidateState(ScreenMirroringState.Playing);

            return RunAsync(Native.Pause, "Failed to pause.");
        }

        /// <summary>
        /// Resumes mirroring from the source.
        /// </summary>
        /// <remarks>
        /// The state must be <see cref="ScreenMirroringState.Paused"/> state by
        /// <see cref="PauseAsync"/>.
        /// </remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <privilege>http://tizen.org/privilege/internet</privilege>
        /// <exception cref="InvalidOperationException">
        ///     The current state is not in the valid.<br/>
        ///     -or-<br/>
        ///     An internal error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <exception cref="UnauthorizedAccessException">Caller does not have required permission.</exception>
        /// <since_tizen> 4 </since_tizen>
        public Task ResumeAsync()
        {
            ValidateState(ScreenMirroringState.Paused);

            return RunAsync(Native.Resume, "Failed to resume.");
        }

        /// <summary>
        /// Disconnects from the source.
        /// </summary>
        /// <remarks>
        /// The state must be <see cref="ScreenMirroringState.Connected"/>,
        /// <see cref="ScreenMirroringState.Playing"/> or <see cref="ScreenMirroringState.Paused"/>.
        /// </remarks>
        /// <privilege>http://tizen.org/privilege/internet</privilege>
        /// <exception cref="InvalidOperationException">
        ///     The current state is not in the valid.<br/>
        ///     -or-<br/>
        ///     An internal error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <exception cref="UnauthorizedAccessException">Caller does not have required permission.</exception>
        /// <since_tizen> 4 </since_tizen>
        public void Disconnect()
        {
            ValidateState(ScreenMirroringState.Connected, ScreenMirroringState.Playing,
                ScreenMirroringState.Paused);

            Native.Disconnect(Handle).ThrowIfError("Failed to disconnect.");
        }

        /// <summary>
        /// Unprepares the screen mirroring.
        /// </summary>
        /// <remarks>
        /// The state must be <see cref="ScreenMirroringState.Prepared"/>,
        /// or <see cref="ScreenMirroringState.Disconnected"/>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     The current state is not in the valid.<br/>
        ///     -or-<br/>
        ///     An internal error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <since_tizen> 4 </since_tizen>
        public void Unprepare()
        {
            ValidateState(ScreenMirroringState.Prepared, ScreenMirroringState.Disconnected);

            Native.Unprepare(Handle).ThrowIfError("Failed to reset.");

            DetachDisplay();
        }

        /// <summary>
        /// Sets the information for UIBC(User Input Back Channel).
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The current state is not in the valid.<br/>
        ///     -or-<br/>
        ///     An internal error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetUibcInformation(Size windowSize, ScreenMirroringCaptureMode mode)
        {
            ValidateState(ScreenMirroringState.Idle);

            Native.SetWindowSize(Handle, windowSize.Width, windowSize.Height).ThrowIfError("Failed to set uibc window size");
            Native.EnableUibc(Handle, mode).ThrowIfError("Failed to set uibc capture mode");
        }

        /// <summary>
        /// Sends mouse event for UIBC(User Input Back Channel).
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The current state is not in the valid.<br/>
        ///     -or-<br/>
        ///     An internal error occurs.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="uibcMouseInfos"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SendGenericMouseEvent(IEnumerable<UibcMouseInfo> uibcMouseInfos, ScreenMirroringMouseEventType type)
        {
            ValidateState(ScreenMirroringState.Connected);

            if (!uibcMouseInfos.Any())
            {
                throw new ArgumentNullException(nameof(uibcMouseInfos));
            }

            var uibcMouseInfosSize = uibcMouseInfos.Count();
            var uibcMouse = new Native.UibcMouse[uibcMouseInfosSize];
            int i = 0;
            IntPtr unmanagedUibcMousePtr;

            foreach (var uibcMouseInfo in uibcMouseInfos)
            {
                uibcMouse[i].id = uibcMouseInfo.Id;
                uibcMouse[i].x = uibcMouseInfo.X;
                uibcMouse[i++].y = uibcMouseInfo.Y;
            }

            var size = Marshal.SizeOf(typeof(Native.UibcMouse));
            IntPtr unmanagedUibcMouse = Marshal.AllocHGlobal(size * uibcMouseInfosSize);
            for (i = 0; i < uibcMouseInfosSize; i++)
            {
                if (IntPtr.Size == 4)
                {
                    unmanagedUibcMousePtr = new IntPtr(unmanagedUibcMouse.ToInt32() + i * size);
                }
                else
                {
                    unmanagedUibcMousePtr = new IntPtr(unmanagedUibcMouse.ToInt64() + i * size);
                }
                Marshal.StructureToPtr(uibcMouse[i], unmanagedUibcMousePtr, false);
            }

            Native.UibcMouseEvent uibcObject;
            uibcObject.size = uibcMouseInfosSize;
            uibcObject.type = type;
            uibcObject.uibcMouse = unmanagedUibcMouse;

            var unmanagedUibcObject = Marshal.AllocHGlobal(Marshal.SizeOf(uibcObject));
            Marshal.StructureToPtr(uibcObject, unmanagedUibcObject, false);

            try
            {
                Native.SendGenericMouseEvent(Handle, unmanagedUibcObject).ThrowIfError("Failed to send generic mouse event");
            }
            finally
            {
                Marshal.FreeHGlobal(unmanagedUibcMouse);
                Marshal.FreeHGlobal(unmanagedUibcObject);
            }
        }

        /// <summary>
        /// Sends key event for UIBC(User Input Back Channel).
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SendGenericKeyEvent(ScreenMirroringKeyEventType type, ushort keyCode1, ushort keyCode2)
        {
            ValidateState(ScreenMirroringState.Connected);

            Native.SendGenericKeyEvent(Handle, type, keyCode1, keyCode2).ThrowIfError("Failed to send generic key event");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ScreenMirroring));
            }
        }

        private ScreenMirroringDisplayMode _displayMode;
        /// <summary>
        /// Gets or sets the display mode.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <since_tizen> 13 </since_tizen>
        public ScreenMirroringDisplayMode DisplayMode
        {
            get
            {
                ThrowIfDisposed();

                return _displayMode;
            }
            set
            {
                Native.SetDisplayMode(Handle, value).ThrowIfError("Failed to set display mode");
                _displayMode = value;
            }
        }

        private Rectangle _displayRoi;
        /// <summary>
        /// Gets or sets the display position and size of the receiver screen view.
        /// </summary>
        /// <remarks>
        /// DisplayRoi will be applied when <see cref="DisplayMode"/> is <see cref="ScreenMirroringDisplayMode.CustomRoi"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     DisplayRoi.X or DisplayRoi.Y is less than 0.<br/>
        ///     -or-<br/>
        ///     DisplayRoi.Width or DisplayRoi.Height is less than or equal to 0.<br/>
        /// </exception>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <seealso cref="DisplayMode"/>
        /// <since_tizen> 13 </since_tizen>
        public Rectangle DisplayRoi
        {
            get
            {
                ThrowIfDisposed();

                return _displayRoi;
            }
            set
            {
                if (value.X < 0)
                {
                    throw new ArgumentOutOfRangeException("X", value.X,
                        $"The X of the roi can't be less than zero.");
                }
                if (value.Y < 0)
                {
                    throw new ArgumentOutOfRangeException("Y", value.Y,
                        $"The Y of the roi can't be less than zero.");
                }
                if (value.Width <= 0)
                {
                    throw new ArgumentOutOfRangeException("Width", value.Width,
                        $"The Width of the roi can't be less than or equal to zero.");
                }
                if (value.Height <= 0)
                {
                    throw new ArgumentOutOfRangeException("Height", value.Height,
                        $"The Height of the roi can't be less than or equal to zero.");
                }

                Native.SetDisplayRoi(Handle, value.X, value.Y, value.Width, value.Height)
                    .ThrowIfError("Failed to set display ROI");
                _displayRoi = value;
            }
        }

        private Rotation _displayRotation;
        /// <summary>
        /// Gets or sets the display rotation.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <since_tizen> 13 </since_tizen>
        public Rotation DisplayRotation
        {
            get
            {
                ThrowIfDisposed();

                return _displayRotation;
            }
            set
            {
                Native.SetDisplayRotation(Handle, value).ThrowIfError("Failed to set display rotation");
                _displayRotation = value;
            }
        }

        private ScreenMirroringDeviceType _srcDeviceType;
        /// <summary>
        /// Gets or sets the source device type.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="ScreenMirroring"/> has already been disposed.</exception>
        /// <since_tizen> 13 </since_tizen>
        public ScreenMirroringDeviceType SourceDeviceType
        {
            get
            {
                ThrowIfDisposed();

                return _srcDeviceType;
            }
            set
            {
                Native.SetSrcDeviceType(Handle, value).ThrowIfError("Failed to set source device type");
                _srcDeviceType = value;
            }
        }

        /// <summary>
        /// Releases all resource used by the <see cref="ScreenMirroring"/> object.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Dispose()"/> when you are finished using the <see cref="ScreenMirroring"/>.
        /// The <see cref="Dispose()"/> method leaves the <see cref="ScreenMirroring"/> in an unusable
        /// state. After calling <see cref="Dispose()"/>, you must release all references to the
        /// <see cref="ScreenMirroring"/> so the garbage collector can reclaim the memory that the
        /// <see cref="ScreenMirroring"/> was occupying.
        /// </remarks>
        /// <since_tizen> 4 </since_tizen>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the resources used by the ScreenMirroring.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        /// <since_tizen> 4 </since_tizen>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                DetachDisplay();

                if (_handle != IntPtr.Zero)
                {
                    Native.Destroy(_handle);
                    _handle = IntPtr.Zero;
                }

                _disposed = true;
            }
        }

        private void RegisterStateChangedEvent()
        {
            _stateChangedCallback = (error, state, _) =>
            {
                var prevState = _state.Value;

                _state.Value = state;

                if (prevState != state)
                {
                    StateChanged?.Invoke(this, new ScreenMirroringStateChangedEventArgs(state));

                }

                if (error != ScreenMirroringErrorCode.None)
                {
                    ErrorOccurred?.Invoke(this, new ScreenMirroringErrorOccurredEventArgs(
                        ScreenMirroringError.InvalidOperation));
                }
            };

            Native.SetStateChangedCb(Handle, _stateChangedCallback).
                ThrowIfError("Failed to initialize StateChanged event.");
        }

        private void RegisterDisplayOrientationChangedEvent()
        {
            _displayOrientationReceivedCallback = (orientation, _) =>
            {
                DisplayOrientationChanged?.Invoke(this, new ScreenMirroringDisplayOrientationChangedEventArgs(orientation));
            };

            Native.SetSrcDisplayOrientationChangedCb(Handle, _displayOrientationReceivedCallback).
                ThrowIfError("Failed to initialize DisplayOrientationChanged event.");
        }

        private void RegisterUibcInfoReceivedEvent()
        {
            _uibcInfoReceivedCallback = (error, uibcInfo, _) =>
            {
                UibcInfoReceived?.Invoke(this, new ScreenMirroringUibcInfoReceivedEventArgs(error.ToCsharp(), uibcInfo));
            };

            Native.SetUibcInfoReceivedCb(Handle, _uibcInfoReceivedCallback).
                ThrowIfError("Failed to initialize UibcInfoReceived event.");
        }

        private void ValidateState(params ScreenMirroringState[] required)
        {
            Debug.Assert(required.Length > 0);

            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ScreenMirroring));
            }

            var curState = _state.Value;
            if (!required.Contains(curState))
            {
                throw new InvalidOperationException($"The screen mirroring is not in a valid state. " +
                    $"Current State : {curState}, Valid State : {string.Join(", ", required)}.");
            }
        }
    }

    internal class AtomicState
    {
        private int _value;

        public AtomicState()
        {
            _value = (int)ScreenMirroringState.Idle;
        }

        public ScreenMirroringState Value
        {
            get
            {
                return (ScreenMirroringState)Interlocked.CompareExchange(ref _value, 0, 0);
            }
            set
            {
                Interlocked.Exchange(ref _value, (int)value);
            }
        }

        public bool IsOneOf(params ScreenMirroringState[] states)
        {
            return states.Contains(Value);
        }
    }
}
