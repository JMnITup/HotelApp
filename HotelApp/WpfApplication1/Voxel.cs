// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Voxel.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#region

using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;
using HotelCorp.HotelApp.Services.Managers;

#endregion

namespace HotelCorp.HotelApp {
    public class Voxel {
        public Voxel() {}

        public Voxel(Point3D position, Color colour, double scale = 1.00, Guest guest = null, string roomNumber = null) {
            Position = position;
            Colour = colour;
            Scale = scale;
            Guest = guest;
            RoomNumber = roomNumber;
        }

        [XmlAttribute("Position")]
        public string XmlPosition {
            get { return Position.ToString(); }
            set { Position = Point3D.Parse(value.Replace(';', ',')); }
        }

        [XmlAttribute("Colour")]
        public string XmlColour {
            get { return Colour.ToString(); }
            set {
                object obj = ColorConverter.ConvertFromString(value);
                if (obj != null) {
                    Colour = (Color) obj;
                }
            }
        }

        [XmlAttribute("Scale")]
        public string XmlScale {
            get { return Scale.ToString(); }
            set { Scale = Convert.ToDouble(value); }
        }

        [XmlIgnore]
        public Point3D Position { get; set; }

        [XmlIgnore]
        public Color Colour { get; set; }

        [XmlIgnore]
        public double Scale { get; set; }

        [XmlIgnore]
        public Guest Guest { get; set; }

        [XmlIgnore]
        public string RoomNumber { get; set; }
    }
}