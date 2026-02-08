/*
 * NUIApplication Test subpage: vertical layout with ScrollableBase and many buttons.
 */

using System;
using System.Reflection;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace NUICoreTest
{
    public static class MainLoopTest
    {
        // NUIApplication test variables
        private static NUIApplication testApp;
        
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
            backButton.Clicked += (s, e) => { window.Remove(subpage); };

            var title = new TextLabel
            {
                Text = "NUIApplication Test",
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

            // NUIApplication Test Buttons
            var createAppButton = new Button
            {
                Text = "Create NUIApplication",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            createAppButton.Clicked += (s, e) => {
                bool result = CreateAppTest();
                createAppButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(createAppButton);

            var registerAssemblyButton = new Button
            {
                Text = "Register Assembly",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            registerAssemblyButton.Clicked += (s, e) => {
                bool result = RegisterAssemblyTest();
                registerAssemblyButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(registerAssemblyButton);

            var addIdleButton = new Button
            {
                Text = "Add Idle",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            addIdleButton.Clicked += (s, e) => {
                bool result = AddIdleTest();
                addIdleButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(addIdleButton);

            var getScreenSizeButton = new Button
            {
                Text = "Get Screen Size",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            getScreenSizeButton.Clicked += (s, e) => {
                bool result = GetScreenSizeTest();
                getScreenSizeButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(getScreenSizeButton);

            var isGeometryHittestEnabledButton = new Button
            {
                Text = "Is Geometry Hittest Enabled",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            isGeometryHittestEnabledButton.Clicked += (s, e) => {
                bool result = IsGeometryHittestEnabledTest();
                isGeometryHittestEnabledButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(isGeometryHittestEnabledButton);

            var setGeometryHittestEnabledButton = new Button
            {
                Text = "Set Geometry Hittest Enabled",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            setGeometryHittestEnabledButton.Clicked += (s, e) => {
                bool result = SetGeometryHittestEnabledTest();
                setGeometryHittestEnabledButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(setGeometryHittestEnabledButton);

            var getAvailableScreensButton = new Button
            {
                Text = "Get Available Screens",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            getAvailableScreensButton.Clicked += (s, e) => {
                bool result = GetAvailableScreensTest();
                getAvailableScreensButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(getAvailableScreensButton);

            var getDefaultWindowButton = new Button
            {
                Text = "Get Default Window",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            getDefaultWindowButton.Clicked += (s, e) => {
                bool result = GetDefaultWindowTest();
                getDefaultWindowButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(getDefaultWindowButton);

            var isUsingXamlButton = new Button
            {
                Text = "Check Is Using Xaml",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            isUsingXamlButton.Clicked += (s, e) => {
                bool result = IsUsingXamlTest();
                isUsingXamlButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(isUsingXamlButton);

            var exitButton = new Button
            {
                Text = "Exit NUIApplication Test",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            exitButton.Clicked += (s, e) => {
                Tizen.Log.Info("NUICoreTest", "Exit NUIApplication Test clicked");
                exitButton.BackgroundColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
            };
            verticalLayout.Add(exitButton);

            scrollable.ContentContainer.Add(verticalLayout);
            subpage.Add(scrollable);

            return subpage;
        }

        // NUIApplication test methods
        private static bool CreateAppTest()
        {
            try
            {
                // Clean up previous app if exists
                if (testApp != null)
                {
                    testApp.Exit();
                    testApp = null;
                }
                
                // Create new NUIApplication
                testApp = new NUIApplication();
                Tizen.Log.Info("NUICoreTest", "NUIApplication created successfully");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to create NUIApplication: {ex.Message}");
                return false;
            }
        }

        private static bool RegisterAssemblyTest()
        {
            try
            {
                if (testApp != null)
                {
                    // Register current assembly
                    NUIApplication.RegisterAssembly(Assembly.GetExecutingAssembly());
                    Tizen.Log.Info("NUICoreTest", "Assembly registered successfully");
                    return true;
                }
                else
                {
                    Tizen.Log.Error("NUICoreTest", "NUIApplication is not created");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to register assembly: {ex.Message}");
                return false;
            }
        }

        private static bool AddIdleTest()
        {
            try
            {
                if (testApp != null)
                {
                    // Add a simple idle function
                    bool result = testApp.AddIdle(new Action(() => {
                        Tizen.Log.Info("NUICoreTest", "Idle function executed");
                    }));
                    
                    if (result)
                    {
                        Tizen.Log.Info("NUICoreTest", "Idle function added successfully");
                        return true;
                    }
                    else
                    {
                        Tizen.Log.Error("NUICoreTest", "Failed to add idle function");
                        return false;
                    }
                }
                else
                {
                    Tizen.Log.Error("NUICoreTest", "NUIApplication is not created");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to add idle function: {ex.Message}");
                return false;
            }
        }

        private static bool GetScreenSizeTest()
        {
            try
            {
                // Get screen size
                Size screenSize = NUIApplication.GetScreenSize();
                Tizen.Log.Info("NUICoreTest", $"Screen size: {screenSize.Width} x {screenSize.Height}");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to get screen size: {ex.Message}");
                return false;
            }
        }

        private static bool IsGeometryHittestEnabledTest()
        {
            try
            {
                // Check if geometry hittest is enabled
                bool isEnabled = NUIApplication.IsGeometryHittestEnabled();
                Tizen.Log.Info("NUICoreTest", $"Geometry hittest enabled: {isEnabled}");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to check geometry hittest: {ex.Message}");
                return false;
            }
        }

        private static bool SetGeometryHittestEnabledTest()
        {
            try
            {
                // Set geometry hittest enabled
                NUIApplication.SetGeometryHittestEnabled(true);
                Tizen.Log.Info("NUICoreTest", "Geometry hittest enabled successfully");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to enable geometry hittest: {ex.Message}");
                return false;
            }
        }

        private static bool GetAvailableScreensTest()
        {
            try
            {
                // Get available screens
                ScreenInformation[] screens = NUIApplication.GetAvailableScreens();
                Tizen.Log.Info("NUICoreTest", $"Number of available screens: {screens?.Length ?? 0}");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to get available screens: {ex.Message}");
                return false;
            }
        }

        private static bool GetDefaultWindowTest()
        {
            try
            {
                // Get default window
                Window window = NUIApplication.GetDefaultWindow();
                Tizen.Log.Info("NUICoreTest", $"Default window retrieved: {window != null}");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to get default window: {ex.Message}");
                return false;
            }
        }

        private static bool IsUsingXamlTest()
        {
            try
            {
                // Check if XAML is being used
                bool isUsingXaml = NUIApplication.IsUsingXaml;
                Tizen.Log.Info("NUICoreTest", $"Is using XAML: {isUsingXaml}");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to check if using XAML: {ex.Message}");
                return false;
            }
        }
    }
}
