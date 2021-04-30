
using global::System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using NUnit.Framework;

namespace Tizen.NUI.Samples
{
    using log = Tizen.Log;
    public class GLWindowTest : IExample
    {
        const string lib = "libdali-nativegl-library.so.1";
        [global::System.Runtime.InteropServices.DllImport(lib, EntryPoint = "init_gl")]
        public static extern void init_gl();

        [global::System.Runtime.InteropServices.DllImport(lib, EntryPoint = "draw_gl")]
        public static extern int draw_gl();

        [global::System.Runtime.InteropServices.DllImport(lib, EntryPoint = "del_gl")]
        public static extern void del_gl();

        public GLWindow mGLWindow;
        public int touchCount;
        string tag = "NUITEST";

        void Initialize()
        {
          touchCount = 0;
          mGLWindow = new GLWindow();
          mGLWindow.SetEglConfig(true, true, 0, GLWindow.GLESVersion.Version_3_0);
          mGLWindow.RegisterGlCallback(init_gl, draw_gl, del_gl);
          //mGLWindow.RenderingMode = GLWindow.GLWindowRenderingMode.OnDemand;
          //mGLWindow.SetRenderingMode(NativeGLRendeingMode.OnDemand);

          mGLWindow.Resized += OnResizedEvent;
          mGLWindow.KeyEvent += OnKeyEvent;
          mGLWindow.TouchEvent += OnTouchEvent;

          mGLWindow.Show();

          mGLWindow.RenderOnce();

          // Add GLWindow Avaialble Orientations
          List<GLWindow.GLWindowOrientation> orientations = new List<GLWindow.GLWindowOrientation>();
          orientations.Add(GLWindow.GLWindowOrientation.Portrait);
          orientations.Add(GLWindow.GLWindowOrientation.Landscape);
          orientations.Add(GLWindow.GLWindowOrientation.PortraitInverse);
          orientations.Add(GLWindow.GLWindowOrientation.LandscapeInverse);
          mGLWindow.SetAvailableOrientations(orientations);
          log.Fatal(tag, "animation play!");
        }
        public void OnKeyEvent(object sender, GLWindow.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down)
            {
                if (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape")
                {
                    Exit();
                }
                else if (e.Key.KeyPressedName == "1")
                {
                    Rectangle rect = new Rectangle(0, 0, 320, 240);

                    mGLWindow.WindowPositionSize = rect;
                }
                // else if (e.Key.KeyPressedName == "2")
                // {
                //   if( mGLWindow.RenderingMode ==  GLWindow.GLWindowRenderingMode.OnDemand)
                //   {
                //     mGLWindow.RenderingMode = GLWindow.GLWindowRenderingMode.Continuous;
                //   }
                //   else if(mGLWindow.RenderingMode ==  GLWindow.GLWindowRenderingMode.Continuous)
                //   {
                //     mGLWindow.RenderingMode = GLWindow.GLWindowRenderingMode.OnDemand;
                //   }
                // }
                // else if (e.Key.KeyPressedName == "3")
                // {
                //   if( mGLWindow.RenderingMode ==  GLWindow.GLWindowRenderingMode.OnDemand)
                //   {
                //     mGLWindow.RenderOnce();
                //   }
                // }
            }
        }
        public void OnTouchEvent(object sender, GLWindow.TouchEventArgs e)
        {
           if(touchCount < 5)
           {
              mGLWindow.RenderOnce();
              touchCount++;
           }
           else if(touchCount == 5)
           {
              // if(mGLWindow.GetRenderingMode() == NativeGLRendeingMode.OnDemand)
              // mGLWindow.SetRenderingMode(NativeGLRendeingMode.Continuous);
              touchCount = 0;
           }
            //if (e.Touch.GetState(0) == PointStateType.Up)
            //{
            //    update_touch_event_state(2);
           // }
            //else if (e.Touch.GetState(0) == PointStateType.Down)
           // {
           //     update_touch_event_state(1);
           // }
           // else if (e.Touch.GetState(0) == PointStateType.Motion)
           // {
           //     update_touch_position((int)(e.Touch.GetScreenPosition(0).X), (int)(e.Touch.GetScreenPosition(0).Y));
           // }
        }

        public void OnResizedEvent(object sender, GLWindow.ResizedEventArgs e)
        {
            Console.WriteLine("OnResizedEvent w:" + e.WindowSize.Width);
            Console.WriteLine("OnResizedEvent h:" + e.WindowSize.Height);

            int current_angle = ((int)mGLWindow.GetCurrentOrientation()) * 90;
            Console.WriteLine("OnResizedEvent current angle:" + current_angle);
            //update_window_rotation_angle(current_angle);
            //update_window_size(e.WindowSize.Width, e.WindowSize.Height);
        }

        public void Activate() 
        { 
            Initialize(); 
        }
        public void Deactivate() 
        { 

        }
    }
}
