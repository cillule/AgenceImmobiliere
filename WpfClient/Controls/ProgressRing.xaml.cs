using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Oyosoft.AgenceImmobiliere.WpfClient.Controls
{
    /// <summary>
    /// Logique d'interaction pour ProgressRing.xaml
    /// </summary>
    public partial class ProgressRing : UserControl
    {
        public static readonly DependencyProperty ParticleColorProperty = DependencyProperty.RegisterAttached("ParticleColor", typeof(Color), typeof(ProgressRing), new UIPropertyMetadata(Colors.Black));
        public Color ParticleColor
        {
            get { return (Color)GetValue(ParticleColorProperty); }
            set { SetValue(ParticleColorProperty, value); }
        }

        public static readonly DependencyProperty ParticleOpacityProperty = DependencyProperty.RegisterAttached("ParticleOpacity", typeof(double), typeof(ProgressRing), new UIPropertyMetadata(1.0));
        public double ParticleOpacity
        {
            get { return Convert.ToDouble(GetValue(ParticleOpacityProperty)); }
            set { SetValue(ParticleOpacityProperty, value); }
        }

        public static readonly DependencyProperty ParticleRadiusProperty = DependencyProperty.RegisterAttached("ParticleRadius", typeof(double), typeof(ProgressRing), new UIPropertyMetadata(5.0));
        public double ParticleRadius
        {
            get { return Convert.ToDouble(GetValue(ParticleRadiusProperty)); }
            set { SetValue(ParticleRadiusProperty, value); }
        }

        public ProgressRing()
        {
            InitializeComponent();
        }
    }
}
