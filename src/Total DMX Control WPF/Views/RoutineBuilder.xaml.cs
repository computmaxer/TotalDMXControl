using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Total_DMX_Control_WPF
{
    /// <summary>
    /// Interaction logic for SubroutineBuilder.xaml
    /// </summary>
    public partial class RoutineBuilder : Window
    {
        //Setup the view
        //The routine!
        private Routine _routine;
        //Toolbar
        private List<ToggleButton> toolbarButtons;
        private ToggleButton _activeTool;
        //Canvas
        private StepShape _stepShape;
        private RoutineFixtureReferencePoint _refPoint;
        private AttributePoint _attrPoint;
        private bool _drawing;
        private bool _movingShape;
        private bool _movingReferencePoint;
        private bool _movingAttributePoint;
        private bool _makingPath;
        private bool _makingPathStep2;
        private Point _last_mouse_location;
        private AttributePointPopup attributePointPopup;
        //Fixture Timeline
        private AddFixturePopup addFixturePop;
        private RoutineFixture _lastSelectedFixture;
        //Step List
        private RoutineStep _lastSelectedStep;


        #region Properties
        public bool LivePreview
        {
            get { return (bool)GetValue(LivePreviewProperty); }
            set { SetValue(LivePreviewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LivePreview.
        public static readonly DependencyProperty LivePreviewProperty =
                DependencyProperty.Register("LivePreview", typeof(bool), typeof(RoutineBuilder), new UIPropertyMetadata(false));

        public Canvas Canvas
        {
            get { return canvas1; }
        }
        #endregion

        public RoutineBuilder(Routine routine)
        {
            //View initialization
            InitializeComponent();
            LivePreview = false;
            //Initialize Routine
            _routine = routine;
            _routine.RoutineBuilder = this;
            tbxRoutineName.Text = _routine.Name;
            this.Title = _routine.Name;
            //Build Canvas toolbar - drawing shapes icons
            toolbarButtons = new List<ToggleButton>();
            toolbarButtons.Add(btnToolbarMove);
            toolbarButtons.Add(btnToolbarArc);
            toolbarButtons.Add(btnToolbarCircle);
            toolbarButtons.Add(btnToolbarDot);
            toolbarButtons.Add(btnToolbarLine);
            toolbarButtons.Add(btnToolbarPolyline);
            toolbarButtons.Add(btnToolbarRectangle);
            toolbarButtons.Add(btnToolbarReferencePoint);
            toolbarButtons.Add(btnToolbarAttrPoint);
            _activeTool = null;
            _drawing = false;
            _movingShape = false;
            _movingReferencePoint = false;
            _movingAttributePoint = false;
            _makingPath = false;
            _makingPathStep2 = false;
            attributePointPopup = new AttributePointPopup();
            //Build "Add Fixture" popup
            addFixturePop = new AddFixturePopup();
            addFixturePop.AddSelectedClick += new RoutedEventHandler(SubroutineBuilder_AddSelectedClick);

            //Initialize Timeline
            CollectionContainer items = new CollectionContainer();
            items.Collection = _routine.RoutineFixtures;
            CollectionContainer newFixtureLineCollection = new CollectionContainer();
            BindingList<string> newFixtureLine = new BindingList<string>();
            newFixtureLine.Add("Click here to add a fixture...");
            newFixtureLineCollection.Collection = newFixtureLine;

            CompositeCollection cmpc = new CompositeCollection();
            cmpc.Add(items);
            cmpc.Add(newFixtureLineCollection);

            lbxTimeline.ItemsSource = cmpc;
            //lbxTimeline.ItemsSource = _routine.RoutineFixtures;
            _lastSelectedFixture = null;

            //Step items list. necessary?
            lbxSteps.ItemsSource = null;

            //Reference Point list.
            lbxReferencePoints.ItemsSource = null;
        }

        /*
         * CANVAS/DRAWING
         */
        private void canvas1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Must have left the canvas while drawing and now they clicking to stop darwing?
            if (_drawing)
                return;

            if (_makingPathStep2)
            {
                _makingPathStep2 = false;
                _makingPath = false;
                return;
            }

            Point p = e.GetPosition((UIElement)sender);

            HitTestResult result = VisualTreeHelper.HitTest(canvas1, p);
            
            //Check to make sure there is a Fixture selected in the Timeline
            if (lbxTimeline.SelectedItem == null || !(lbxTimeline.SelectedItem is RoutineFixture))
            {
                if (_activeTool == null) return;
                if(lbxTimeline.Items.Count > 0)
                    MessageBox.Show("You must select a fixture to add the step to.");
                else
                    MessageBox.Show("You must add a fixture first.");
                return;
            }

            //Move an attribute point after it has already been placed.
            if (result.VisualHit.GetType() == typeof(Ellipse))
            {
                if (((Ellipse)result.VisualHit).Name.Contains("_attrPoint"))
                {
                    _attrPoint = GetAttributePointEllipseParent((Ellipse)result.VisualHit);
                    _movingAttributePoint = true;
                    return;
                }
            }

            //The move tool. :) For moving a shape.
            if (_activeTool == btnToolbarMove)
            {
                Shape closestShape = (Shape)GetClosestCanvasChild(p);
                StepShape stepShape = GetShapeParent(closestShape);
                _movingShape = true;
                _stepShape = stepShape;
                _stepShape.Select();
                foreach (UIElement element in canvas1.Children)
                {
                    if (element.GetType() == typeof(Shape))
                    {
                        if (((Shape)element).Name.Contains("_stepShape"))
                        {
                            StepShape parent = GetShapeParent((Shape)element);
                            parent.Deselect();
                        }
                    }
                }
            }

            if (_stepShape == null || result.VisualHit != _stepShape.Shape)
            {
                //This probably shouldn't get here. I don't know where it should go yet
                if(_stepShape != null) _stepShape.Deselect();
                
                //If user has selected a shape tool.
                if (_activeTool != null && (_activeTool == btnToolbarCircle || _activeTool == btnToolbarDot || _activeTool == btnToolbarLine
                    || _activeTool == btnToolbarPolyline || _activeTool == btnToolbarRectangle || _activeTool == btnToolbarArc))
                {
                    _drawing = true;
                    RemoveCurrentShape();
                    if (_activeTool == btnToolbarCircle)
                    {
                        Ellipse ell = (Ellipse)StepEllipse.Draw(e.GetPosition(canvas1).X, e.GetPosition(canvas1).Y);
                        ell.Name += "_stepShape";
                        canvas1.Children.Add(ell);
                        _stepShape = new StepEllipse(ell);
                    }
                    else if (_activeTool == btnToolbarRectangle)
                    {
                        Rectangle rect = StepRectangle.Draw(e.GetPosition(canvas1).X, e.GetPosition(canvas1).Y);
                        rect.Name += "_stepShape";
                        canvas1.Children.Add(rect);
                        _stepShape = new StepRectangle(rect);
                    }
                    else if (_activeTool == btnToolbarLine)
                    {
                        Line line = StepLine.Draw(e.GetPosition(canvas1).X, e.GetPosition(canvas1).Y);
                        line.Name += "_stepShape";
                        canvas1.Children.Add(line);
                        _stepShape = new StepLine(line);
                    }
                    else if (_activeTool == btnToolbarArc)
                    {
                        _stepShape = new StepPath(e.GetPosition(canvas1).X, e.GetPosition(canvas1).Y);
                        _stepShape.Shape.Name += "_stepShape";
                        canvas1.Children.Add(_stepShape.Shape);
                        _makingPath = true;
                    }
                    else if (_activeTool == btnToolbarPolyline)
                    {
                        _stepShape = new StepFreeForm(e.GetPosition(canvas1).X, e.GetPosition(canvas1).Y);
                        _stepShape.Shape.Name += "_stepShape";
                        canvas1.Children.Add(_stepShape.Shape);
                    }
                    _last_mouse_location = e.GetPosition(canvas1);
                    //Create new list item for the step shape & add to list.
                    RoutineStep rs = new RoutineStep("New Step", _stepShape, _routine);
                    ((RoutineFixture)lbxTimeline.SelectedItem).RoutineSteps.Add(rs);
                    _lastSelectedStep = rs;
                    /*Horrible way to refresh the listbox.  Need to figure this out.
                     * Currently it removes the row that allows you to add more fixutres. Can't do that!
                     */
                    //lbxSteps.ItemsSource = null;
                    //lbxSteps.Items.Clear();
                    //lbxSteps.ItemsSource = ((RoutineFixture)lbxTimeline.SelectedItem).RoutineSteps;
                }
                //Not drawing something, making a reference point?
                else if(_activeTool == btnToolbarReferencePoint)
                {
                    _movingReferencePoint = true;
                    _refPoint = new RoutineFixtureReferencePoint((RoutineFixture)lbxTimeline.SelectedItem, e.GetPosition(canvas1));
                    canvas1.Children.Add(_refPoint.Ellipse);
                    canvas1.Children.Add(_refPoint.Label);
                    _refPoint.MoveFixtureTo();
                    ((RoutineFixture)lbxTimeline.SelectedItem).Fixture.On();
                }
                //Not a referencePoint, is it an Attribute Point?
                else if (_activeTool == btnToolbarAttrPoint)
                {
                    _movingAttributePoint = true;
                    
                    Shape closestShape = (Shape)GetClosestCanvasChild(p);
                    Point closestPoint = GetClosestPointOnShape(closestShape, p);

                    _attrPoint = new AttributePoint(closestPoint, ((RoutineFixture)lbxTimeline.SelectedItem).Fixture);
                    canvas1.Children.Add(_attrPoint.Ellipse);
                    Canvas.SetLeft(_attrPoint.Ellipse, closestPoint.X -450);
                    Canvas.SetTop(_attrPoint.Ellipse, closestPoint.Y -450);
                    _lastSelectedStep.AddAttributePoint(_attrPoint);
                    
                    //Live Preview
                    if (LivePreview)
                    {
                        ((RoutineFixture)lbxTimeline.SelectedItem).Fixture.MoveTo(closestPoint, 0);
                    }
                }
            }

        }

        private void canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_drawing)
            {
                _stepShape.DrawMouseMove((e.GetPosition(canvas1).X - _last_mouse_location.X),(e.GetPosition(canvas1).Y - _last_mouse_location.Y));
                _last_mouse_location = e.GetPosition(canvas1);
            }
            else if(_movingShape)
            {
                _stepShape.MoveShape(e.GetPosition(canvas1).X - _last_mouse_location.X, e.GetPosition(canvas1).Y - _last_mouse_location.Y);
                _last_mouse_location = e.GetPosition(canvas1);
            }
            else if (_movingReferencePoint)
            {
                _refPoint.Move(e.GetPosition(canvas1).X, e.GetPosition(canvas1).Y);
                _refPoint.MoveFixtureTo();
            }
            else if (_movingAttributePoint)
            {
                Point p = e.GetPosition(canvas1);
                Shape closestShape = (Shape)GetClosestCanvasChild(p);
                Point closestPoint = GetClosestPointOnShape(closestShape, p);
                _attrPoint.Point = closestPoint;
                Canvas.SetLeft(_attrPoint.Ellipse, closestPoint.X - 450);
                Canvas.SetTop(_attrPoint.Ellipse, closestPoint.Y - 450);
                if (LivePreview)
                {
                    ((RoutineFixture)lbxTimeline.SelectedItem).Fixture.MoveTo(closestPoint, 0);
                }
            }
            else if (_makingPathStep2)
            {
                _stepShape.DrawStep2MouseMove(e.GetPosition(canvas1).X - _last_mouse_location.X, e.GetPosition(canvas1).Y - _last_mouse_location.Y);
                _last_mouse_location = e.GetPosition(canvas1);
            }
        }

        private void canvas1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_movingReferencePoint)
            {
                ((RoutineFixture)lbxTimeline.SelectedItem).Fixture.Off();
            }
            else if (_movingAttributePoint)
            {
                attributePointPopup.Control.lbxAttributePoint.ItemsSource = _attrPoint.AttributePointSettings;
                attributePointPopup.HorizontalOffset = this.Left + (_attrPoint.Point.X / 120.468) + 51;
                attributePointPopup.VerticalOffset = this.Top + (_attrPoint.Point.Y / 120.468) + 40;
                attributePointPopup.Show(PlacementMode.RelativePoint);
                
            }
            else if (_makingPath)
            {
                _makingPathStep2 = true;
            }
            _drawing = false;
            _movingShape = false;
            _movingReferencePoint = false;
            _movingAttributePoint = false;
            if (_stepShape != null)
            {
                _stepShape.UpdateBoundingBox();
            }
            _last_mouse_location = new Point(0,0);
        }

        private void lbxSteps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveCurrentShape();
            if (lbxSteps.SelectedItem != null)
            {
                ((RoutineStep)lbxSteps.SelectedItem).AddToCanvas(canvas1);
                _lastSelectedStep = ((RoutineStep)lbxSteps.SelectedItem);
            }
        }

        private void RemoveCurrentShape()
        {
            if (_lastSelectedStep != null)
            {
                _lastSelectedStep.RemoveFromCanvas(canvas1);
            }
        }

        // PREVIEW STEP!
        private void btnPreviewStep_Click(object sender, RoutedEventArgs e)
        {
            RoutineStepPlayer player = new RoutineStepPlayer();
            if (lbxSteps.SelectedItem != null && lbxTimeline.SelectedItem != null)
            {
                player.PreviewStep(((RoutineFixture)lbxTimeline.SelectedItem).Fixture, (RoutineStep)lbxSteps.SelectedItem);
            }
        }

        //Toolbar click handlers
        private void ToggleToolbarButton(ToggleButton but)
        {
            _activeTool = but;
            foreach (ToggleButton tb in toolbarButtons)
            {
                if (but == tb)
                {
                    tb.IsChecked = true;
                }
                else
                {
                    tb.IsChecked = false;
                }
            }
            //if (_activeTool == btnToolbarMove)
            //{
            //    if(_stepShape != null)
            //        _stepShape.BindEvents();
            //}
            //else
            //{
            //    if(_stepShape != null)
            //        _stepShape.UnbindEvents();
            //}
        }

        private void btnToolbar_Click(object sender, RoutedEventArgs e)
        {
            ToggleToolbarButton((ToggleButton)sender);
        }

        /*
         * TIMELINE
         */
        private void lbxTimeline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_lastSelectedFixture != null)
            {
                _lastSelectedFixture.RemoveFromCanvas(canvas1);
            }

   
            if (lbxTimeline.SelectedItem.GetType() == typeof(string))  // hacky way of testing if user clicked "add new fixture"
            {
                addFixturePop.Show(PlacementMode.Mouse);

                lbxSteps.ItemsSource = null;
                lbxReferencePoints.ItemsSource = null;
            }
            else
            {
                RoutineFixture rf = ((RoutineFixture)lbxTimeline.SelectedItem);

                lbxReferencePoints.ItemsSource = rf.ReferencePoints;
                if (lbxSteps.ItemsSource == null)
                {
                    lbxSteps.Items.Clear();
                }

                lbxSteps.ItemsSource = rf.RoutineSteps;
                if (_lastSelectedStep != null)
                    _lastSelectedStep.RemoveFromCanvas(canvas1);

                _lastSelectedFixture = rf;
                _lastSelectedFixture.AddToCanvas(canvas1);

            }
            
        }

        private void SubroutineBuilder_AddSelectedClick(object sender, RoutedEventArgs e)
        {
            foreach (Fixture fixt in ((AddFixturePopupControl)((Border)addFixturePop.Child).Child).lbxFixturesToAdd.SelectedItems)
            {
                _routine.RoutineFixtures.Add(new RoutineFixture(fixt));
            }
            addFixturePop.Hide();
        }

        private void lbxiAddFixture_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void btnAddFixture_Click(object sender, RoutedEventArgs e)
        {
            //temp
            addFixturePop.Show(PlacementMode.Mouse);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _routine.Name = tbxRoutineName.Text;
            Controller.Routines.Add(_routine);
            this.Close();
        }

        private void btnPreviewRoutine_Click(object sender, RoutedEventArgs e)
        {
            RoutinePlayer player = new RoutinePlayer();
            player.PreviewRoutine(_routine);
        }

        //Helpers
        private UIElement GetClosestCanvasChild(Point p)
        {
            double distance = 66000;
            UIElement closest = null;
            foreach (UIElement child in canvas1.Children)
            {
                try
                {
                    if (!((Shape)child).Name.Contains("_stepShape"))
                    {
                        continue;
                    }
                }
                catch(Exception e)
                {
                    continue;
                }
                double thisDistance = 66000;
                if (child.GetType() == typeof(Polyline))
                {
                    thisDistance = Utilities.GetDistanceBetweenPoints(((Polyline)child).Points[0], p);
                }
                else
                {
                    thisDistance = Utilities.GetDistanceBetweenPoints(new Point(Canvas.GetLeft(child), Canvas.GetTop(child)), p);
                }
                if (thisDistance < distance)
                {
                    distance = thisDistance;
                    closest = child;
                }
            }
            return closest;
        }

        private Point GetClosestPointOnShape(Shape shape, Point pt)
        {
            StepShape stepShape = GetShapeParent(shape);
            List<Point> points = stepShape.GetPoints(GetStepShapeParent(stepShape).Duration);

            return Utilities.GetClosestPointInList(pt, points);
        }

        private StepShape GetShapeParent(Shape shape)
        {
            foreach (RoutineStep step in ((RoutineFixture)lbxTimeline.SelectedItem).RoutineSteps)
            {
                if (step.StepShape.Shape == shape) return step.StepShape;
            }
            return null;
        }

        private RoutineStep GetStepShapeParent(StepShape stepShape)
        {
            foreach (RoutineStep step in ((RoutineFixture)lbxTimeline.SelectedItem).RoutineSteps)
            {
                if (step.StepShape == stepShape) return step;
            }
            return null;
        }

        private AttributePoint GetAttributePointEllipseParent(Ellipse ellipse)
        {
            foreach (AttributePoint attrPoint in _lastSelectedStep.AttrPointsOnlyUseForLoopingThroughNotAdding)
            {
                if (attrPoint.Ellipse == ellipse) return attrPoint;
            }
            return null;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            foreach (RoutineFixture fix in _routine.RoutineFixtures)
            {
                fix.Fixture.Reset();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            addFixturePop.Hide();
            addFixturePop = null;
            attributePointPopup.Hide();
            attributePointPopup = null;

            _routine.RoutineBuilder = null;
            _routine.DetachStepsFromCanvas(Canvas);
        }

        private void DeleteStep_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((RoutineStep)lbxSteps.SelectedItem).Visible = false;
            ((RoutineStep)lbxSteps.SelectedItem).RemoveFromCanvas(canvas1);
            ((RoutineFixture)lbxTimeline.SelectedItem).RoutineSteps.Remove((RoutineStep)lbxSteps.SelectedItem);
        }

        private void tbxRoutineName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _routine.Name = tbxRoutineName.Text;
            this.Title = tbxRoutineName.Text;
        }
    }
    
}
