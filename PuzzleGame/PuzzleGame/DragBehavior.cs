using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataAnalysis;

namespace PuzzleGame
{
    public class DragBehavior : Behavior<UIElement>
    {
        private Point elementStartPosition;
        private Point trueStartPosition;
        private Point mouseStartPosition;
        private Point trueTranslate;
        private RotateTransform rotate = new RotateTransform(90);
        private TranslateTransform translate = new TranslateTransform();
        private TransformGroup group = new TransformGroup();

        public bool Horizontal
        {
            get
            {
                foreach (var el in group.Children)
                {
                    if (el is RotateTransform)
                        return true;
                }

                return false;
            }
        }

        protected override void OnAttached()
        {
            Window parent = Application.Current.MainWindow;
            var h = ((ContentControl)parent.Content).ActualHeight;
            var w = ((ContentControl)parent.Content).ActualWidth;
            AssociatedObject.RenderTransform = group;
            AssociatedObject.RenderTransformOrigin = new Point(0.5, 0.5);
            group.Children.Add(translate);
            trueTranslate.X = translate.X;
            trueTranslate.Y = translate.Y;

            Match match = (Match)AssociatedObject;

            AssociatedObject.MouseLeftButtonDown += (sender, e) =>
            {
                if (match.Puzzle.MatchesToMoveLeft < 1 && !match.WasMoved)
                    return;
                match.Slot.ContentMatch = null;
                if (e.ClickCount == 2)
                {
                    if (Horizontal)
                    {
                        foreach (var el in group.Children)
                        {
                            if (el is RotateTransform)
                            {
                                group.Children.Remove(el);
                                break;
                            }
                        }

                        translate.X = trueTranslate.X;
                        translate.Y = trueTranslate.Y;
                    }
                    else
                    {
                        rotate.CenterX = translate.X;
                        rotate.CenterY = translate.Y;
                        group.Children.Add(rotate);
                    }
                    match.Horizontal = !match.Horizontal;
                    // call rotate
                }
                else
                {
                    mouseStartPosition = parent.PointToScreen(e.GetPosition(parent));
                    AssociatedObject.CaptureMouse();
                    // call pick
                    DataWriter.DataConsumer("Object_get", System.DateTime.Now, (int)trueTranslate.X, (int)trueTranslate.Y, match.Id);
                }  
            };

            AssociatedObject.MouseLeftButtonUp += (sender, e) =>
            {
                if (match.Puzzle.MatchesToMoveLeft < 1 && !match.WasMoved)
                    return;
                AssociatedObject.ReleaseMouseCapture();
                elementStartPosition.X = translate.X;
                elementStartPosition.Y = translate.Y;
                trueStartPosition.X = trueTranslate.X;
                trueStartPosition.Y = trueTranslate.Y;
                match.TrueOffsetX = trueTranslate.X;
                match.TrueOffsetY = trueTranslate.Y;
                System.Diagnostics.Trace.WriteLine((int)(match.X + match.RealX) + "; " + (int)(match.Y + match.RealY));
                match.AttachToSlot();
                System.Diagnostics.Trace.WriteLine((int)(match.X + match.RealX) + "; " + (int)(match.Y + match.RealY));
                // call drop
                DataWriter.DataConsumer("Object_loose", System.DateTime.Now, (int)trueTranslate.X, (int)trueTranslate.Y, match.Id);
            };

            AssociatedObject.MouseMove += (sender, e) =>
            {
                if (match.Puzzle.MatchesToMoveLeft < 1 && !match.WasMoved)
                    return;
                var parentPos = parent.PointToScreen(e.GetPosition(parent));
                Vector diff = (parentPos - mouseStartPosition);
                if (AssociatedObject.IsMouseCaptured)
                {
                    if (Horizontal)
                    {
                        translate.X = elementStartPosition.X + diff.Y / 1.25;
                        translate.Y = elementStartPosition.Y - diff.X / 1.25;
                        trueTranslate.X = trueStartPosition.X + diff.X / 1.25;
                        trueTranslate.Y = trueStartPosition.Y + diff.Y / 1.25;
                    }
                    else
                    {
                        translate.X = elementStartPosition.X + diff.X / 1.25;
                        translate.Y = elementStartPosition.Y + diff.Y / 1.25;
                        trueTranslate.X = translate.X;
                        trueTranslate.Y = translate.Y;
                    }
                }
            };
        }
    }
}
