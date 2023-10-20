using Microsoft.Maui.Controls.Shapes;

namespace Windows_Shadow_Mask_Bug {
    public partial class App : Application {
        public App() {
            InitializeComponent();

            ContentPage mainPage = new();
            mainPage.BackgroundColor = Colors.LightPink;
            this.MainPage = mainPage;
            
            ScreenSizeMonitor.Instance.setContentPage(mainPage);
            
            VerticalStackLayout vert = new();
            mainPage.Content = vert;

            AbsoluteLayout abs = new();
            vert.Add(abs);
            abs.Add(TopObjectWithShadow.Instance);

        }
    }
    public class TopObjectWithShadow : AbsoluteLayout {
        public static TopObjectWithShadow Instance { get { return lazy.Value; } }
        private static readonly Lazy<TopObjectWithShadow> lazy = new Lazy<TopObjectWithShadow>(() => new TopObjectWithShadow());

        public TopSubComponent topMenuFrameComponent;

        private TopObjectWithShadow() {

            this.IgnoreSafeArea = true;
            topMenuFrameComponent = new();
            this.Add(topMenuFrameComponent);
            topMenuFrameComponent.Shadow = new() { Offset = new Point(0, 5), Radius = 5 };
            
            ScreenSizeMonitor.Instance.ScreenSizeChanged += screenSizeChanged;
        }

        public void screenSizeChanged() {
            if (topMenuFrameComponent != null) {
                this.WidthRequest = ScreenSizeMonitor.Instance.screenWidth;
                this.HeightRequest = ScreenSizeMonitor.Instance.screenHeight;

                topMenuFrameComponent.resizeFunction();
            }
        }
    }

    public class TopSubComponent : AbsoluteLayout {
        Border frameBorder;
        double frameBaseHeight = 68;

        public TopSubComponent() {
            frameBorder = new();
            this.Add(frameBorder);
            this.IgnoreSafeArea = true;
            frameBorder.HeightRequest = frameBaseHeight;
            frameBorder.BackgroundColor = Colors.White;
            frameBorder.Margin = new Thickness(0, 20, 0, 0);
            frameBorder.StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(30, 30, 30, 30) };
            frameBorder.StrokeThickness = 0;
        }
        public void resizeFunction() {
            this.WidthRequest = ScreenSizeMonitor.Instance.screenWidth;
            this.HeightRequest = ScreenSizeMonitor.Instance.screenHeight;
            frameBorder.WidthRequest = ScreenSizeMonitor.Instance.screenWidth;
            frameBorder.HeightRequest = frameBaseHeight;
        }
    }
    public class ScreenSizeMonitor {

        public ContentPage pageToMonitor;

        //SINGLETON LAZY PATTERN 
        private static readonly Lazy<ScreenSizeMonitor> lazy = new Lazy<ScreenSizeMonitor>(() => new ScreenSizeMonitor());
        public static ScreenSizeMonitor Instance { get { return lazy.Value; } }

        //SCREEN CHANGE FUNCTIONS 
        public double screenWidth = 0;
        public double screenHeight = 0;
        public event Action ScreenSizeChanged = null;

        public void setContentPage(ContentPage contentPageToMonitor) {
            pageToMonitor = contentPageToMonitor;
            startScreenMonitor();
        }
        private void startScreenMonitor() {

            updateFunction();
            pageToMonitor.SizeChanged += delegate {
                updateFunction();
            };

        }
        private void updateFunction() {
            if (pageToMonitor.Width > 0 && pageToMonitor.Height > 0) {
                screenWidth = pageToMonitor.Width;
                screenHeight = pageToMonitor.Height;

                invokeScreenSizeChanged();
            }
        }
        public void invokeScreenSizeChanged() {
            MainThread.BeginInvokeOnMainThread(() => { 
                ScreenSizeChanged?.Invoke();
            });
        }
    }
}