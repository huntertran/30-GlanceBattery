using System.Windows.Controls;

namespace KaraokeOnline.CustomControl
{
    public partial class SpinningSquare : UserControl
    {
        public SpinningSquare()
        {
            InitializeComponent();
            SpinningSquareStoryBoard.Begin();
        }
    }
}
