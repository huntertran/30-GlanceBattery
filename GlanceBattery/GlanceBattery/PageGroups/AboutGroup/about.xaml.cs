using Microsoft.Phone.Controls;
using System.Windows.Media;

namespace KaraokeOnline.PageGroups.AboutGroup
{
    public partial class about : PhoneApplicationPage
    {
        private TranslateTransform translateTransform;

        public about()
        {
            InitializeComponent();
            translateTransform = new TranslateTransform();
            spinningSquare.RenderTransform = translateTransform;
        }

        private void GestureListener_DragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            translateTransform.X += e.HorizontalChange;
            translateTransform.Y += e.VerticalChange;
        }
    }
}