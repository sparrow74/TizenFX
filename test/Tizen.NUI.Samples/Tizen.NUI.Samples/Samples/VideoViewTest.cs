
using global::System;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Tizen.NUI.Samples
{
    using tlog = Tizen.Log;

    public class VideoViewTest : IExample
    {
        Window win;
        myPlayer player;
        string resourcePath1;
        string resourcePath2;         
        const string tag = "NUI";
        View buttonContainer;

        VideoView videoView1;
        VideoView videoView2;

        Animation animation1;
        Animation animation2;

        Button playVideo;
        Button pauseVideo;
        Button moveVideo;
        Button raiseTop;
        Button lowerBottom;
        Button raiseAbove;
        Button lowerBelow;

        bool   positionChanged;

        // Make derieved class from Tizen.Multimedia.Player because protected Player(IntPtr handle, Action<int, string> errorHandler)
        // this constructor's access modifyer is protected, so there is no other way.
        public class myPlayer : Tizen.Multimedia.Player
        {
            public myPlayer() : base()
            {
                //Initialize();
            }

            public myPlayer(IntPtr p) : base(p, null)
            {
                //Initialize();
            }
        }

        Window win;
        myPlayer player;
        string resourcePath;
        const string tag = "NUITEST";
        View dummy;
        public void Activate()
        {
            win = NUIApplication.GetDefaultWindow();
            win.BackgroundColor = new Color(1, 1, 1, 0);
            win.KeyEvent += Win_KeyEvent;
            win.TouchEvent += Win_TouchEvent;

            resourcePath1 = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "v.mp4";
            tlog.Fatal(tag, $"resourcePath1: {resourcePath1}");
            resourcePath2 = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "test.mp4";
            tlog.Fatal(tag, $"resourcePath1: {resourcePath1}");

            
            //PlayerTest();

            NUIVideoViewButton();

            NUIVideoView1Test();
            NUIVideoView2Test();

            animation1 = new Animation(0);
            animation2 = new Animation(0);

            positionChanged = false;
        }
        public void Deactivate()
        {
            win.KeyEvent -= Win_KeyEvent;
            win.TouchEvent -= Win_TouchEvent;

            tlog.Fatal(tag, $"Deactivate()!");
            videoView1.Stop();
            videoView1.Unparent();
            videoView1.Dispose();
            videoView1 = null;

            videoView2.Stop();
            videoView2.Unparent();
            videoView2.Dispose();
            videoView2 = null;

            buttonContainer.Unparent();
            buttonContainer.Dispose();
            buttonContainer = null;

            // currently it is crashed when Dispose() is called. need to check.

            //player.Unprepare();
            //player.Dispose();
            //player = null;

            tlog.Fatal(tag, $"Deactivate()! videoView dispsed");
        }

        int cnt;
        private void Win_TouchEvent(object sender, Window.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
                if (++cnt % 2 == 1)
                {
                    if (player != null)
                    {
                        player.Pause();
                        tlog.Fatal(tag, $"player pause!");
                    }
                }
                else
                {
                    if (player != null)
                    {
                        player.Start();
                        tlog.Fatal(tag, $"player start!");
                    }
                }
            }
        }

        public async void PlayerTest()
        {
            player = new myPlayer();

            player.SetSource(new Tizen.Multimedia.MediaUriSource(resourcePath1));

            player.Display = new Tizen.Multimedia.Display(win);

            await player.PrepareAsync().ConfigureAwait(false);
            tlog.Fatal(tag, $"await player.PrepareAsync();");

            player.Start();
            tlog.Fatal(tag, $"player.Start();");

            if (player.DisplaySettings.IsVisible == false)
            {
                player.DisplaySettings.IsVisible = true;
            }
            tlog.Fatal(tag, $"Display visible = {player.DisplaySettings.IsVisible}");

            player.DisplaySettings.Mode = Tizen.Multimedia.PlayerDisplayMode.FullScreen;
        }

        public void NUIVideoView1Test()
        {
            videoView1 = new VideoView(VideoView.VideoSyncMode.Enabled);
            videoView1.Underlay = true;
            videoView1.ResourceUrl = resourcePath1;
            videoView1.Looping = true;
            videoView1.Size = new Size(640, 400);
            videoView1.PositionUsesPivotPoint = true;
            videoView1.ParentOrigin = Tizen.NUI.ParentOrigin.CenterLeft;
            videoView1.PivotPoint = Tizen.NUI.PivotPoint.CenterLeft;
            videoView1.PositionUsesPivotPoint = true;
            win.Add(videoView1);

            //var playerHandle = new SafeNativePlayerHandle(videoView1);
            //player = new myPlayer(playerHandle.DangerousGetHandle());
            //if (player != null)
            //{
            //    player.Start();
            //}
        }

        public void NUIVideoView2Test()
        {
            videoView2 = new VideoView(VideoView.VideoSyncMode.Enabled);
            videoView2.Underlay = true;
            videoView2.ResourceUrl = resourcePath2;
            videoView2.Looping = true;
            videoView2.Size = new Size(640, 400);
            videoView2.PositionUsesPivotPoint = true;
            videoView2.ParentOrigin = Tizen.NUI.ParentOrigin.CenterRight;
            videoView2.PivotPoint = Tizen.NUI.PivotPoint.CenterRight;
            videoView2.PositionUsesPivotPoint = true;
            win.Add(videoView2);

            //var playerHandle = new SafeNativePlayerHandle(videoView2);
            //player = new myPlayer(playerHandle.DangerousGetHandle());
            //if (player != null)
            //{
            //    player.Start();
            //}
        }

        private void PlayVideo_Clicked(object sender, ClickedEventArgs e)
        {
            tlog.Fatal(tag, $"moveVideo_Clicked()!");
            videoView1.Play();
            videoView2.Play();
        }

        private void PauseVideo_Clicked(object sender, ClickedEventArgs e)
        {
            tlog.Fatal(tag, $"moveVideo_Clicked()!");
            videoView1.Pause();
            videoView2.Pause();
        }

        private void MoveVideo_Clicked(object sender, ClickedEventArgs e)
        {
            tlog.Fatal(tag, $"moveVideo_Clicked()!");

            if(positionChanged == false)
            {
              animation1.AnimateTo(videoView1, "positionX", 1920.0f, 0, 6000, new AlphaFunction(AlphaFunction.BuiltinFunctions.Default)); 
              animation2.AnimateTo(videoView2, "positionX", -1920.0f, 0, 6000, new AlphaFunction(AlphaFunction.BuiltinFunctions.Default)); 
              positionChanged = true;
            }
            else
            {
              animation1.AnimateTo(videoView1, "positionX", 0.0f, 0, 6000, new AlphaFunction(AlphaFunction.BuiltinFunctions.Default)); 
              animation2.AnimateTo(videoView2, "positionX", 0.0f, 0, 6000, new AlphaFunction(AlphaFunction.BuiltinFunctions.Default)); 
              positionChanged = false;
            }
            
            videoView1.PlayAnimation(animation1);
            videoView2.PlayAnimation(animation2);
        }

        private void RaiseTop_Clicked(object sender, ClickedEventArgs e)
        {
            tlog.Fatal(tag, $"moveVideo_Clicked()!");
            videoView1.RaiseToTop();
        }

        private void LowerBottom_Clicked(object sender, ClickedEventArgs e)
        {
            tlog.Fatal(tag, $"moveVideo_Clicked()!");
            videoView1.LowerToBottom();
        }

        private void RaiseToAbove_Clicked(object sender, ClickedEventArgs e)
        {
            tlog.Fatal(tag, $"moveVideo_Clicked()!");
            videoView1.RaiseAbove(videoView2);
        }

        private void LowerToBelow_Clicked(object sender, ClickedEventArgs e)
        {
            tlog.Fatal(tag, $"moveVideo_Clicked()!");
            videoView1.LowerBelow(videoView2);
        }

        public void NUIVideoViewButton()
        { 
            buttonContainer = new View()
            {
                Layout = new LinearLayout()
                {
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                }
            };
            buttonContainer.ParentOrigin = Tizen.NUI.ParentOrigin.TopCenter;
            buttonContainer.PivotPoint = Tizen.NUI.PivotPoint.TopCenter;
            buttonContainer.PositionUsesPivotPoint = true;

            //Play Button
            var playVideoStyle = new ButtonStyle()
            {
                Overlay = new ImageViewStyle()
                {
                    ResourceUrl = new Selector<string>
                    {
                        Pressed = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_press_overlay.png",
                        Other = ""
                    },
                    Border = new Rectangle(5, 5, 5, 5)
                },
                Text = new TextLabelStyle()
                {
                    TextColor = new Selector<Color>
                    {
                        Normal = new Color(1, 1, 1, 1),
                        Pressed = new Color(1, 1, 1, 0.7f),
                        Disabled = new Color(1, 1, 1, 0.4f)
                    },
                    Text = "Play",
                    PointSize = 15,
                },
                BackgroundImage = CommonResource.GetFHResourcePath() + "3. Button/rectangle_point_btn_normal.png",
                BackgroundImageBorder = new Rectangle(5, 5, 5, 5),
            };
            playVideo = new Button();
            playVideo.ApplyStyle(playVideoStyle);
            playVideo.ImageShadow = new ImageShadow
            {
                Url = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_shadow.png",
                Border = new Rectangle(5, 5, 5, 5)
            };
            playVideo.Size = new Size(200, 60);
            playVideo.Clicked += PlayVideo_Clicked;
            buttonContainer.Add(playVideo);

            //Pause Button
            var pauseVideoStyle = new ButtonStyle()
            {
                Overlay = new ImageViewStyle()
                {
                    ResourceUrl = new Selector<string>
                    {
                        Pressed = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_press_overlay.png",
                        Other = ""
                    },
                    Border = new Rectangle(5, 5, 5, 5)
                },
                Text = new TextLabelStyle()
                {
                    TextColor = new Selector<Color>
                    {
                        Normal = new Color(1, 1, 1, 1),
                        Pressed = new Color(1, 1, 1, 0.7f),
                        Disabled = new Color(1, 1, 1, 0.4f)
                    },
                    Text = "Pause",
                    PointSize = 15,
                },
                BackgroundImage = CommonResource.GetFHResourcePath() + "3. Button/rectangle_point_btn_normal.png",
                BackgroundImageBorder = new Rectangle(5, 5, 5, 5),
            };
            pauseVideo = new Button();
            pauseVideo.ApplyStyle(pauseVideoStyle);
            pauseVideo.ImageShadow = new ImageShadow
            {
                Url = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_shadow.png",
                Border = new Rectangle(5, 5, 5, 5)
            };
            pauseVideo.Size = new Size(200, 60);
            pauseVideo.Clicked += PauseVideo_Clicked;
            buttonContainer.Add(pauseVideo);

            //Move Button
            var moveVideoStyle = new ButtonStyle()
            {
                Overlay = new ImageViewStyle()
                {
                    ResourceUrl = new Selector<string>
                    {
                        Pressed = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_press_overlay.png",
                        Other = ""
                    },
                    Border = new Rectangle(5, 5, 5, 5)
                },
                Text = new TextLabelStyle()
                {
                    TextColor = new Selector<Color>
                    {
                        Normal = new Color(1, 1, 1, 1),
                        Pressed = new Color(1, 1, 1, 0.7f),
                        Disabled = new Color(1, 1, 1, 0.4f)
                    },
                    Text = "Move",
                    PointSize = 15,
                },
                BackgroundImage = CommonResource.GetFHResourcePath() + "3. Button/rectangle_point_btn_normal.png",
                BackgroundImageBorder = new Rectangle(5, 5, 5, 5),
            };
            moveVideo = new Button();
            moveVideo.ApplyStyle(moveVideoStyle);
            moveVideo.ImageShadow = new ImageShadow
            {
                Url = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_shadow.png",
                Border = new Rectangle(5, 5, 5, 5)
            };
            moveVideo.Size = new Size(200, 60);
            moveVideo.Clicked += MoveVideo_Clicked;
            buttonContainer.Add(moveVideo);

            //RaiseTop Button
            var raiseTopButtonStyle = new ButtonStyle()
            {
                Overlay = new ImageViewStyle()
                {
                    ResourceUrl = new Selector<string>
                    {
                        Pressed = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_press_overlay.png",
                        Other = ""
                    },
                    Border = new Rectangle(5, 5, 5, 5)
                },
                Text = new TextLabelStyle()
                {
                    TextColor = new Selector<Color>
                    {
                        Normal = new Color(1, 1, 1, 1),
                        Pressed = new Color(1, 1, 1, 0.7f),
                        Disabled = new Color(1, 1, 1, 0.4f)
                    },
                    Text = "RaiseTop",
                    PointSize = 15,
                },
                BackgroundImage = CommonResource.GetFHResourcePath() + "3. Button/rectangle_point_btn_normal.png",
                BackgroundImageBorder = new Rectangle(5, 5, 5, 5)
            };
            raiseTop = new Button();
            raiseTop.ApplyStyle(raiseTopButtonStyle);
            raiseTop.ImageShadow = new ImageShadow
            {
                Url = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_shadow.png",
                Border = new Rectangle(5, 5, 5, 5)
            };

            raiseTop.Size = new Size(200, 60);
            raiseTop.Clicked += RaiseTop_Clicked;
            buttonContainer.Add(raiseTop);

            //LowerBottom Button
            var lowerBottomButtonStyle = new ButtonStyle()
            {
                Overlay = new ImageViewStyle()
                {
                    ResourceUrl = new Selector<string>
                    {
                        Pressed = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_press_overlay.png",
                        Other = ""
                    },
                    Border = new Rectangle(5, 5, 5, 5)
                },
                Text = new TextLabelStyle()
                {
                    TextColor = new Selector<Color>
                    {
                        Normal = new Color(1, 1, 1, 1),
                        Pressed = new Color(1, 1, 1, 0.7f),
                        Disabled = new Color(1, 1, 1, 0.4f)
                    },
                    Text = "LowerBottom",
                    PointSize = 15,
                },
                BackgroundImage = CommonResource.GetFHResourcePath() + "3. Button/rectangle_point_btn_normal.png",
                BackgroundImageBorder = new Rectangle(5, 5, 5, 5),
            };
            lowerBottom = new Button();
            lowerBottom.ApplyStyle(lowerBottomButtonStyle);
            lowerBottom.ImageShadow = new ImageShadow
            {
                Url = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_shadow.png",
                Border = new Rectangle(5, 5, 5, 5)
            };
            lowerBottom.Size = new Size(200, 60);
            lowerBottom.Clicked += LowerBottom_Clicked;
            buttonContainer.Add(lowerBottom);

            var raiseAboveButtonStyle = new ButtonStyle()
            {
                Overlay = new ImageViewStyle()
                {
                    ResourceUrl = new Selector<string>
                    {
                        Pressed = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_press_overlay.png",
                        Other = ""
                    },
                    Border = new Rectangle(5, 5, 5, 5)
                },
                Text = new TextLabelStyle()
                {
                    TextColor = new Selector<Color>
                    {
                        Normal = new Color(1, 1, 1, 1),
                        Pressed = new Color(1, 1, 1, 0.7f),
                        Disabled = new Color(1, 1, 1, 0.4f)
                    },
                    Text = "RaiseAbove",
                    PointSize = 15,
                },
                BackgroundImage = CommonResource.GetFHResourcePath() + "3. Button/rectangle_point_btn_normal.png",
                BackgroundImageBorder = new Rectangle(5, 5, 5, 5)
            };
            raiseAbove = new Button();
            raiseAbove.ApplyStyle(raiseAboveButtonStyle);
            raiseAbove.ImageShadow = new ImageShadow
            {
                Url = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_shadow.png",
                Border = new Rectangle(5, 5, 5, 5)
            };

            raiseAbove.Size = new Size(200, 60);
            raiseAbove.Clicked += RaiseToAbove_Clicked;
            buttonContainer.Add(raiseAbove);

            var lowerBelowButtonStyle = new ButtonStyle()
            {
                Overlay = new ImageViewStyle()
                {
                    ResourceUrl = new Selector<string>
                    {
                        Pressed = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_press_overlay.png",
                        Other = ""
                    },
                    Border = new Rectangle(5, 5, 5, 5)
                },
                Text = new TextLabelStyle()
                {
                    TextColor = new Selector<Color>
                    {
                        Normal = new Color(1, 1, 1, 1),
                        Pressed = new Color(1, 1, 1, 0.7f),
                        Disabled = new Color(1, 1, 1, 0.4f)
                    },
                    Text = "LowerBelow",
                    PointSize = 15,
                },
                BackgroundImage = CommonResource.GetFHResourcePath() + "3. Button/rectangle_point_btn_normal.png",
                BackgroundImageBorder = new Rectangle(5, 5, 5, 5)
            };
            lowerBelow = new Button();
            lowerBelow.ApplyStyle(lowerBelowButtonStyle);
            lowerBelow.ImageShadow = new ImageShadow
            {
                Url = CommonResource.GetFHResourcePath() + "3. Button/rectangle_btn_shadow.png",
                Border = new Rectangle(5, 5, 5, 5)
            };

            lowerBelow.Size = new Size(200, 60);
            lowerBelow.Clicked += LowerToBelow_Clicked;
            buttonContainer.Add(lowerBelow);

            win.Add(buttonContainer);
        }
    }
}
