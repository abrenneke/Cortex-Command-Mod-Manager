using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;
using Newtonsoft.Json;

namespace CortexCommandModManager.Activities
{
    public enum ActivityItemGroup { Weapon, Actor, Craft }
    public enum ActivityItemType { ACRocket, ACDropShip, HeldDevice, HDFirearm, TDExplosive, AHuman, ACrab }

    [JsonObject(MemberSerialization.OptOut)]
    public class ActivityItem
    {
        public ActivityItemGroup Group { get; set; }
        public ActivityItemType Type { get; set; }

        public Mod Mod { get; set; }

        public string PresetName { get; set; }

        public string Description { get; set; }

        public string SpritePath { get; set; }

        public bool Buyable { get; set; }

        private BitmapSource bitmapSource;

        [JsonIgnore]
        public BitmapSource BitmapSource { get { if (bitmapSource == null) MakeBitmapImage(Grabber.Settings.Get().CCInstallDirectory); return bitmapSource; } }

        public ActivityItem() { Buyable = true; }

        public void MakeBitmapImage(string CCPath)
        {
            if (SpritePath == null)
            {
                this.bitmapSource = null;
            }
            else
            {
                var fullSpritePath = CCPath + "\\" + SpritePath.Replace('/', '\\');
                System.Drawing.Bitmap bitmap = null;
                try
                {
                    bitmap = new System.Drawing.Bitmap(fullSpritePath);
                }
                catch (ArgumentException)
                {
                    try
                    {
                        var spriteInfo = new FileInfo(fullSpritePath);
                        var image1Path = spriteInfo.FullName.Substring(0, spriteInfo.FullName.Length - spriteInfo.Extension.Length) +
                                        "000" + spriteInfo.Extension;
                        bitmap = new System.Drawing.Bitmap(image1Path);
                    }
                    catch (ArgumentException) { return; }
                }
                System.Drawing.Color transparentColor = System.Drawing.ColorTranslator.FromHtml("#FF00FF");
                bitmap.MakeTransparent(transparentColor);
                System.Drawing.Bitmap map = new System.Drawing.Bitmap(bitmap);
                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    map.GetHbitmap(),
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(map.Width, map.Height)
                    );

                this.bitmapSource = bitmapSource;

            }
        }
    }
}
