/* * Screen Test subpage: vertical layout with ScrollableBase and many buttons. */
using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace NUICoreTest
{
    public static class ScreenTest
    {
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
                window.Remove(subpage); 
            };

            var title = new TextLabel
            {
                Text = "Screen Test",
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

            // Screen Test Buttons
            var getScreenInfoButton = new Button
            {
                Text = "Get Screen Info",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            getScreenInfoButton.Clicked += (s, e) => {
                bool result = GetScreenInfoTest();
                getScreenInfoButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(getScreenInfoButton);

            // ScreenInformation unit test buttons
            var constructorTestButton = new Button
            {
                Text = "Constructor Test",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            constructorTestButton.Clicked += (s, e) => {
                bool result = ScreenInformationConstructorTest();
                constructorTestButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(constructorTestButton);

            var equalsTestButton = new Button
            {
                Text = "Equals Test",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            equalsTestButton.Clicked += (s, e) => {
                bool result = ScreenInformationEqualsTest();
                equalsTestButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(equalsTestButton);

            var hashCodeTestButton = new Button
            {
                Text = "GetHashCode Test",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            hashCodeTestButton.Clicked += (s, e) => {
                bool result = ScreenInformationGetHashCodeTest();
                hashCodeTestButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(hashCodeTestButton);

            var operatorTestButton = new Button
            {
                Text = "Operator Test",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            operatorTestButton.Clicked += (s, e) => {
                bool result = ScreenInformationOperatorTest();
                operatorTestButton.BackgroundColor = result ? new Color(0.0f, 0.0f, 1.0f, 1.0f) : new Color(1.0f, 0.0f, 0.0f, 1.0f);
            };
            verticalLayout.Add(operatorTestButton);

            var exitButton = new Button
            {
                Text = "Exit Screen Test",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 56,
            };
            exitButton.Clicked += (s, e) => {
                Tizen.Log.Info("NUICoreTest", "Exit Screen Test clicked");
                exitButton.BackgroundColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
            };
            verticalLayout.Add(exitButton);

            scrollable.ContentContainer.Add(verticalLayout);
            subpage.Add(scrollable);

            return subpage;
        }

        // Screen test methods
        private static bool GetScreenInfoTest()
        {
            try
            {
                // Get screen information
                var screenInfo = Tizen.NUI.ScreenInformation.Get();
                Tizen.Log.Info("NUICoreTest", $"Screen Name: {screenInfo.Name}");
                Tizen.Log.Info("NUICoreTest", $"Screen Width: {screenInfo.Width}");
                Tizen.Log.Info("NUICoreTest", $"Screen Height: {screenInfo.Height}");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"Failed to get screen info: {ex.Message}");
                return false;
            }
        }

        // ScreenInformation unit tests
        private static bool ScreenInformationConstructorTest()
        {
            try
            {
                // Test constructor with valid values
                string testName = "TestScreen";
                int testWidth = 1920;
                int testHeight = 1080;
                
                var screenInfo = new Tizen.NUI.ScreenInformation(testName, testWidth, testHeight);
                
                // Verify properties are set correctly
                if (screenInfo.Name != testName)
                {
                    Tizen.Log.Error("NUICoreTest", "ScreenInformation constructor failed: Name mismatch");
                    return false;
                }
                
                if (screenInfo.Width != testWidth)
                {
                    Tizen.Log.Error("NUICoreTest", "ScreenInformation constructor failed: Width mismatch");
                    return false;
                }
                
                if (screenInfo.Height != testHeight)
                {
                    Tizen.Log.Error("NUICoreTest", "ScreenInformation constructor failed: Height mismatch");
                    return false;
                }
                
                Tizen.Log.Info("NUICoreTest", "ScreenInformation constructor test passed");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"ScreenInformation constructor test failed: {ex.Message}");
                return false;
            }
        }

        private static bool ScreenInformationEqualsTest()
        {
            try
            {
                // Create two ScreenInformation objects with same values
                var screenInfo1 = new Tizen.NUI.ScreenInformation("TestScreen", 1920, 1080);
                var screenInfo2 = new Tizen.NUI.ScreenInformation("TestScreen", 1920, 1080);
                var screenInfo3 = new Tizen.NUI.ScreenInformation("DifferentScreen", 1280, 720);
                
                // Test Equals method
                if (!screenInfo1.Equals(screenInfo2))
                {
                    Tizen.Log.Error("NUICoreTest", "ScreenInformation Equals method failed: Equal objects not recognized");
                    return false;
                }
                
                if (screenInfo1.Equals(screenInfo3))
                {
                    Tizen.Log.Error("NUICoreTest", "ScreenInformation Equals method failed: Different objects recognized as equal");
                    return false;
                }
                
                // Test object.Equals override
                if (!screenInfo1.Equals((object)screenInfo2))
                {
                    Tizen.Log.Error("NUICoreTest", "ScreenInformation object.Equals override failed: Equal objects not recognized");
                    return false;
                }
                
                Tizen.Log.Info("NUICoreTest", "ScreenInformation Equals test passed");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"ScreenInformation Equals test failed: {ex.Message}");
                return false;
            }
        }

        private static bool ScreenInformationGetHashCodeTest()
        {
            try
            {
                // Create two ScreenInformation objects with same values
                var screenInfo1 = new Tizen.NUI.ScreenInformation("TestScreen", 1920, 1080);
                var screenInfo2 = new Tizen.NUI.ScreenInformation("TestScreen", 1920, 1080);
                
                // Test GetHashCode method - equal objects should have same hash code
                if (screenInfo1.GetHashCode() != screenInfo2.GetHashCode())
                {
                    Tizen.Log.Error("NUICoreTest", "ScreenInformation GetHashCode method failed: Equal objects have different hash codes");
                    return false;
                }
                
                Tizen.Log.Info("NUICoreTest", "ScreenInformation GetHashCode test passed");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"ScreenInformation GetHashCode test failed: {ex.Message}");
                return false;
            }
        }

        private static bool ScreenInformationOperatorTest()
        {
            try
            {
                // Create ScreenInformation objects for testing
                var screenInfo1 = new Tizen.NUI.ScreenInformation("TestScreen", 1920, 1080);
                var screenInfo2 = new Tizen.NUI.ScreenInformation("TestScreen", 1920, 1080);
                var screenInfo3 = new Tizen.NUI.ScreenInformation("DifferentScreen", 1280, 720);
                
                // Test == operator
                if (!(screenInfo1 == screenInfo2))
                {
                    Tizen.Log.Error("NUICoreTest", "ScreenInformation == operator failed: Equal objects not recognized");
                    return false;
                }
                
                if (screenInfo1 == screenInfo3)
                {
                    Tizen.Log.Error("NUICoreTest", "ScreenInformation == operator failed: Different objects recognized as equal");
                    return false;
                }
                
                // Test != operator
                if (screenInfo1 != screenInfo2)
                {
                    Tizen.Log.Error("NUICoreTest", "ScreenInformation != operator failed: Equal objects recognized as different");
                    return false;
                }
                
                if (!(screenInfo1 != screenInfo3))
                {
                    Tizen.Log.Error("NUICoreTest", "ScreenInformation != operator failed: Different objects not recognized");
                    return false;
                }
                
                Tizen.Log.Info("NUICoreTest", "ScreenInformation operator test passed");
                return true;
            }
            catch (Exception ex)
            {
                Tizen.Log.Error("NUICoreTest", $"ScreenInformation operator test failed: {ex.Message}");
                return false;
            }
        }
    }
}
