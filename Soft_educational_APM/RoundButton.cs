// RoundButton class adapts the original Button to look round

using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Soft_educational_APM {
	public class RoundButton : Button {
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			GraphicsPath grPath = new GraphicsPath();
			grPath.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
			this.Region = new System.Drawing.Region(grPath);
			base.OnPaint(e);
		}
	}
}
