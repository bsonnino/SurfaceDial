using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SurfaceDial
{

    
    public sealed partial class MainPage : Page
    {
        public enum CurrentTool
        {
            Resize,
            Rotate,
            MoveX,
            MoveY,
            Color
        }

        private CurrentTool _currentTool;
        private readonly List<SolidColorBrush> _namedBrushes;
        private int _selBrush;

        public MainPage()
        {
            this.InitializeComponent();
            // Create a reference to the RadialController.
            var controller = RadialController.CreateForCurrentView();

            // Create the icons for the dial menu
            var iconResize = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Resize.png"));
            var iconRotate = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Rotate.png"));
            var iconMoveX= RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/MoveX.png"));
            var iconMoveY = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/MoveY.png"));
            var iconColor = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Color.png"));

            // Create the items for the menu
            var itemResize = RadialControllerMenuItem.CreateFromIcon("Resize", iconResize);
            var itemRotate = RadialControllerMenuItem.CreateFromIcon("Rotate", iconRotate);
            var itemMoveX = RadialControllerMenuItem.CreateFromIcon("MoveX", iconMoveX);
            var itemMoveY = RadialControllerMenuItem.CreateFromIcon("MoveY", iconMoveY);
            var itemColor = RadialControllerMenuItem.CreateFromIcon("Color", iconColor);

            // Add the items to the menu
            controller.Menu.Items.Add(itemResize);
            controller.Menu.Items.Add(itemRotate);
            controller.Menu.Items.Add(itemMoveX);
            controller.Menu.Items.Add(itemMoveY);
            controller.Menu.Items.Add(itemColor);

            // Select the correct tool when the item is selected
            itemResize.Invoked += (s,e) =>_currentTool = CurrentTool.Resize;
            itemRotate.Invoked += (s,e) =>_currentTool = CurrentTool.Rotate;
            itemMoveX.Invoked += (s,e) =>_currentTool = CurrentTool.MoveX;
            itemMoveY.Invoked += (s,e) =>_currentTool = CurrentTool.MoveY;
            itemColor.Invoked += (s,e) =>_currentTool = CurrentTool.Color;

            // Get all named colors and create brushes from them
            _namedBrushes = typeof(Colors).GetRuntimeProperties().Select(c => new SolidColorBrush((Color)c.GetValue(null))).ToList();
            
            controller.RotationChanged += ControllerRotationChanged;
            
            // Leave only the Volume default item - Zoom and Undo won't be used
            RadialControllerConfiguration config = RadialControllerConfiguration.GetForCurrentView();
            config.SetDefaultMenuItems(new[] { RadialControllerSystemMenuItemKind.Volume });

        }

        private void ControllerRotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            switch (_currentTool)
            {
                case CurrentTool.Resize:
                    Scale.ScaleX += args.RotationDeltaInDegrees / 10;
                    Scale.ScaleY += args.RotationDeltaInDegrees / 10;
                    break;
                case CurrentTool.Rotate:
                    Rotate.Angle += args.RotationDeltaInDegrees;
                    break;
                case CurrentTool.MoveX:
                    Translate.X += args.RotationDeltaInDegrees;
                    break;
                case CurrentTool.MoveY:
                    Translate.Y += args.RotationDeltaInDegrees;
                    break;
                case CurrentTool.Color:
                    _selBrush += (int)(args.RotationDeltaInDegrees / 10);
                    if (_selBrush >= _namedBrushes.Count)
                        _selBrush = 0;
                    if (_selBrush < 0)
                        _selBrush = _namedBrushes.Count-1;
                    Rectangle.Fill = _namedBrushes[(int)_selBrush];
                    break;
                default:
                    break;
            }
            
        }
        
    }
}
