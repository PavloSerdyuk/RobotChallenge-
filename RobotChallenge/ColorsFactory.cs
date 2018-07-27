using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Robot.Common;

namespace RobotChallenge
{
    public class ColorsFactory
    {
        public static Dictionary<Owner, Color> OwnerColors;


        public static void Initialize(List<Owner> owners)
        {

            System.Drawing.Color[] colorsD = {
                                                 System.Drawing.Color.FromArgb(0x1e, 0x90, 0xff),
                                                 System.Drawing.Color.FromArgb(0x00, 0xff, 0xff),
                                                 System.Drawing.Color.FromArgb(0x32, 0xcd, 0x32),
                                                 System.Drawing.Color.FromArgb(0x6b, 0x8e, 0x23),
                                                 System.Drawing.Color.FromArgb(0xbc, 0x8f, 0x8f),
                                                 System.Drawing.Color.FromArgb(0xf0, 0x80, 0x80),
                                                 System.Drawing.Color.FromArgb(0xb2, 0x22, 0x22),
                                                 System.Drawing.Color.FromArgb(0xda, 0xa5, 0x20),
                                                 System.Drawing.Color.FromArgb(0xb8, 0x86, 0x0b),
                                                 System.Drawing.Color.FromArgb(0xff, 0xff, 0x00),
                                                 System.Drawing.Color.FromArgb(0xff, 0x00, 0x00),
                                                 System.Drawing.Color.FromArgb(0x80, 0x00, 0x00),
                                                 System.Drawing.Color.FromArgb(0xc0, 0xc0, 0xc0),
                                                 System.Drawing.Color.FromArgb(0x00, 0x00, 0x00),
                                                 System.Drawing.Color.FromArgb(0x00, 0xff, 0x00),

                                             };
            var colors =
                colorsD.Select(color => new Color() {A = color.A, B = color.B, G = color.G, R = color.R}).ToList();

            colors.Add(Colors.DarkRed);
            colors.Add(Colors.HotPink);
            colors.Add(Colors.CadetBlue);
            colors.Add(Colors.DarkKhaki);
            
            /*                     new Color(){R= },
                                 Colors.Firebrick,
                                 Colors.Aquamarine,
                                 Colors.BurlyWood,
                                 Colors.CadetBlue,
                                 
                                 Colors.CornflowerBlue,
                                 Colors.DarkGoldenrod,
                                 Colors.DarkCyan,
                                 Colors.DarkKhaki,

                                 Colors.DarkRed,
                                 Colors.DeepPink,
                                 Colors.Gray,
                                 Colors.LemonChiffon,
                                 Colors.Olive,
                                 Colors.ForestGreen,
                                 Colors.Fuchsia,

                                 Colors.HotPink,
                                 Colors.DeepSkyBlue,
                                 Colors.Coral*/

            int colorIndex = 0;
            OwnerColors = new Dictionary<Owner, Color>();

            foreach (var owner in owners)
            {
                OwnerColors.Add(owner, colors[colorIndex]);
                colorIndex++;
            }
        }
    }
}
