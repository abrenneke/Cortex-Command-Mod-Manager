using System;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;

namespace CortexCommandModManager
{
    public class Mod : IModListItem, IComparable<Mod>, IComparable
    {
        /// <summary>
        /// Gets the full path to the folder of the mod
        /// </summary>
        public string FullFolderPath { get; set; }

        /// <summary>
        /// The folder the mod is located in
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// Gets the title of the mod
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets whether the mod is enabled. Use Enable() or Disable() to enable or disable the mod.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the icon for the mod, if there is one
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Gets the bitmap image for use in the UI
        /// </summary>
        public BitmapSource BitmapSource { get; private set; }

        public bool IsPreinstalled { get { return PreinstalledMods.IsPreinstalledMod(this); } }

        /// <summary>
        /// Make a mod from the mod details. Mods shouldn't be changed on the fly, so they are created here.
        /// </summary>
        /// <param name="fullFolderPath">The full path to the mod folder</param>
        /// <param name="name">The name of the mod</param>
        /// <param name="enabled">If the mod is enabled or disabled initially</param>
        /// <param name="folder">The folder name of the mod (e.g. base.rte)</param>
        /// <param name="icon">The icon for the mod, if it exists</param>
        public static Mod MakeMod(string fullFolderPath, string name, bool enabled, string folder, string icon )
        {
            Mod mod = new Mod
                          {
                              Name = name,
                              FullFolderPath = fullFolderPath,
                              IsEnabled = enabled,
                              IconPath = icon,
                              Folder = folder
                          };
            return mod;
        }

        public void LoadIcon()
        {
            if (IconPath == null)
            {
                BitmapSource = null;
            }
            else
            {   
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(IconPath);
                System.Drawing.Color transparentColor = System.Drawing.ColorTranslator.FromHtml("#FF00FF");
                bitmap.MakeTransparent(transparentColor);
                System.Drawing.Bitmap map = new System.Drawing.Bitmap(bitmap, new System.Drawing.Size(64, 64));
                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    map.GetHbitmap(), 
                    IntPtr.Zero, 
                    System.Windows.Int32Rect.Empty, 
                    BitmapSizeOptions.FromWidthAndHeight(map.Width, map.Height)
                    );

                BitmapSource = bitmapSource;
                
            }
        }
        
        public bool IsInOriginalInstallation()
        {
            return Constants.OriginalInstallationMods.Any(x => x == Folder);
        }

        public bool IsCreatedSkirmishMod()
        {
            return Folder == EnhancedSkirmish.ActivitiesFolderName;
        }

        private bool CanBeDisabled()
        {
            return !IsInOriginalInstallation() && !IsCreatedSkirmishMod();
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;
            if (!(obj is Mod)) return false;
            return this.FullFolderPath == ((Mod)obj).FullFolderPath;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(Mod other)
        {
            return this.Name.CompareTo(other.Name);
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Mod)) return -1;
            return this.CompareTo((Mod)obj);
        }
    }
}
