/* * Timer Test subpage: vertical layout with ScrollableBase and many buttons. */
using System;
using System.Threading;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace NUICoreTest
{
    public static class TimerTest
    {
        // Timer test variables
        private static Tizen.NUI.Timer testTimer;
        private static int tickCount = 0;
        private static bool tickResult = false;
        
        public static View Create(Window window)
        {
            var subpage = new View
            {
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    CellPadding = new Size2D(0, 16),
                },
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                BackgroundColor = new Color(0.95f, 0.95f, 0.95f, 1.0f),
                Padding = new Extents(32, 32, 32, 32),
            };

            var titleRow = new View
            {
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                    CellPadding = new Size2D(16, 0),
                },
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
            };

            var backButton = new Button
            {
                Text = "Back",
                WidthSpecification = 120,
                HeightSpecification = 60,
            };
            backButton.Clicked += (s, e) => { 
                // Clean up timer resources
                if (testTimer != null)
                {
                    testTimer.Stop();
                    testTimer.Dispose();
                    testTimer = null;
                }
                window.Remove(subpage); 
            };

            var title = new TextLabel
            {
                Text = "Timer Test",
                PointSize = 28,
                VerticalAlignment = VerticalAlignment.Center,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
            };

            titleRow.Add(backButton);
            titleRow.Add(title);
            subpage.Add(titleRow);

            var scrollable = new ScrollableBase
            {
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
            };

            var verticalLayout = new View
            {
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical,
                    CellPadding = new Size2D(0, 12),
                },
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Padding = new Extents(0, 16, 0, 0),
            };

            // Timer Test Buttons
            var createTimerButton = new Button
            {
                Text = "Create Timer (100ms)",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            createTimerButton.Clicked += (s, e) => {
                bool result = CreateTimerTest();
                createTimerButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(createTimerButton);

            var startTimerButton = new Button
            {
                Text = "Start Timer",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            startTimerButton.Clicked += (s, e) => {
                bool result = StartTimerTest();
                startTimerButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(startTimerButton);

            var stopTimerButton = new Button
            {
                Text = "Stop Timer",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            stopTimerButton.Clicked += (s, e) => {
                bool result = StopTimerTest();
                stopTimerButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(stopTimerButton);

            var isRunningTimerButton = new Button
            {
                Text = "Check IsRunning",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            isRunningTimerButton.Clicked += (s, e) => {
                bool result = IsRunningTimerTest();
                isRunningTimerButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(isRunningTimerButton);

            var intervalTimerButton = new Button
            {
                Text = "Set Interval (500ms)",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            intervalTimerButton.Clicked += (s, e) => {
                bool result = IntervalTimerTest();
                intervalTimerButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(intervalTimerButton);

            var tickEventTimerButton = new Button
            {
                Text = "Test Tick Event",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            tickEventTimerButton.Clicked += (s, e) => {
                bool result = TickEventTimerTest();
                tickEventTimerButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(tickEventTimerButton);

            var aliveCountTimerButton = new Button
            {
                Text = "Check Alive Count",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            aliveCountTimerButton.Clicked += (s, e) => {
                bool result = AliveCountTimerTest();
                aliveCountTimerButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(aliveCountTimerButton);

            var disposeTimerButton = new Button
            {
                Text = "Dispose Timer",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            disposeTimerButton.Clicked += (s, e) => {
                bool result = DisposeTimerTest();
                disposeTimerButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(disposeTimerButton);

            var exitButton = new Button
            {
                Text = "Exit Timer Test",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            exitButton.Clicked += (s, e) => {
                Tizen.Log.Info("NUICoreTest", "Exit Timer Test clicked");
                exitButton.BackgroundColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
            };
            verticalLayout.Add(exitButton);

            scrollable.ContentContainer.Add(verticalLayout);
            subpage.Add(scrollable);

            return subpage;
        }

        // Timer test methods
        private static bool CreateTimerTest()
        {
            try
            {
                // Clean up previous timer if exists
                if (testTimer != null)
                {
                    testTimer.Stop();
                    testTimer.Dispose();
                }
                
                // Reset test variables
                tickCount = 0;
                tickResult = false;
                
                // Create new timer with 100ms interval
                testTimer = new Tizen.NUI.Timer(100);
                Tizen.Log.Info("NUICoreTest", "Timer created successfully with 100ms interval");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to create timer: {ex.Message}");
                return false;
            }
        }

        private static bool StartTimerTest()
        {
            try
            {
                if (testTimer != null)
                {
                    testTimer.Start();
                    Tizen.Log.Info("NUICoreTest", "Timer started successfully");
                    return true;
                }
                else
                {
                    Tizen.Log.Error("NUICoreTest", "Timer is not created");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to start timer: {ex.Message}");
                return false;
            }
        }

        private static bool StopTimerTest()
        {
            try
            {
                if (testTimer != null)
                {
                    testTimer.Stop();
                    Tizen.Log.Info("NUICoreTest", "Timer stopped successfully");
                    return true;
                }
                else
                {
                    Tizen.Log.Error("NUICoreTest", "Timer is not created");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to stop timer: {ex.Message}");
                return false;
            }
        }

        private static bool IsRunningTimerTest()
        {
            try
            {
                if (testTimer != null)
                {
                    bool isRunning = testTimer.IsRunning();
                    Tizen.Log.Info("NUICoreTest", $"Timer IsRunning: {isRunning}");
                    return true;
                }
                else
                {
                    Tizen.Log.Error("NUICoreTest", "Timer is not created");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to check timer running state: {ex.Message}");
                return false;
            }
        }

        private static bool IntervalTimerTest()
        {
            try
            {
                if (testTimer != null)
                {
                    // Test getting current interval
                    uint currentInterval = testTimer.Interval;
                    Tizen.Log.Info("NUICoreTest", $"Current timer interval: {currentInterval}ms");
                    
                    // Test setting new interval
                    testTimer.Interval = 500;
                    Tizen.Log.Info("NUICoreTest", "Timer interval set to 500ms");
                    
                    // Verify the new interval
                    uint newInterval = testTimer.Interval;
                    Tizen.Log.Info("NUICoreTest", $"New timer interval: {newInterval}ms");
                    return true;
                }
                else
                {
                    Tizen.Log.Error("NUICoreTest", "Timer is not created");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to test timer interval: {ex.Message}");
                return false;
            }
        }

        private static bool TickEventTimerTest()
        {
            try
            {
                if (testTimer != null)
                {
                    // Reset tick count
                    tickCount = 0;
                    
                    // Subscribe to tick event
                    testTimer.Tick += OnTimerTick;
                    
                    // Start timer for 1 second to count ticks
                    testTimer.Interval = 100; // 100ms
                    testTimer.Start();
                    
                    // Wait for some ticks
                    Thread.Sleep(1000);
                    
                    // Stop timer
                    testTimer.Stop();
                    
                    // Unsubscribe from tick event
                    testTimer.Tick -= OnTimerTick;
                    
                    Tizen.Log.Info("NUICoreTest", $"Timer ticked {tickCount} times in 1 second");
                    return true;
                }
                else
                {
                    Tizen.Log.Error("NUICoreTest", "Timer is not created");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to test timer tick event: {ex.Message}");
                return false;
            }
        }

        private static bool OnTimerTick(object sender, Tizen.NUI.Timer.TickEventArgs e)
        {
            tickCount++;
            Tizen.Log.Info("NUICoreTest", $"Timer tick #{tickCount}");
            
            // Return true to continue timer
            return true;
        }

        private static bool AliveCountTimerTest()
        {
            try
            {
                int aliveCount = Tizen.NUI.Timer.AliveCount;
                Tizen.Log.Info("NUICoreTest", $"Number of alive Timer objects: {aliveCount}");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to get alive count: {ex.Message}");
                return false;
            }
        }

        private static bool DisposeTimerTest()
        {
            try
            {
                if (testTimer != null)
                {
                    testTimer.Dispose();
                    testTimer = null;
                    Tizen.Log.Info("NUICoreTest", "Timer disposed successfully");
                    return true;
                }
                else
                {
                    Tizen.Log.Error("NUICoreTest", "Timer is not created");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to dispose timer: {ex.Message}");
                return false;
            }
        }
    }
}
