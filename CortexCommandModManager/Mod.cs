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

        public Mod() { }
        /// <summary>
        /// Make a mod from the mod details. Mods shouldn't be changed on the fly, so they are created here.
        /// </summary>
        /// <param name="fullFolderPath">The full path to the mod folder</param>
        /// <param name="name">The name of the mod</param>
        /// <param name="enabled">If the mod is enabled or disabled initially</param>
        /// <param name="directory"></param>
        /// <param name="icon">The icon for the mod, if it exists</param>
        /// <returns></returns>
        public static Mod MakeMod(string fullFolderPath, string name, bool enabled, string directory, string icon )
        {
            Mod mod = new Mod
                          {
                              Name = name,
                              FullFolderPath = fullFolderPath,
                              IsEnabled = enabled,
                              IconPath = icon,
                              Folder = directory
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
        public void ToggleEnabled()
        {
            if (IsEnabled)
            {
                Disable();
            }
            else
            {
                Enable();
            }
        }
        public void Enable()
        {
            if (!IsEnabled)
            {
                try
                {
                    string newPath = ModManager.EnableMod(this);
                    IsEnabled = true;
                    FullFolderPath = newPath;
                    IconPath = ModScanner.FindModImagePath(FullFolderPath);
                }
                catch (IOException)
                {
                    throw new ModLockedException("The mod cannot be enabled, as an error occurred. Most likely there is a folder with the same name already enabled.");
                }
            }
        }
        public void Disable()
        {
            if (IsEnabled)
            {
                if (CanBeDisabled())
                {
                    try
                    {
                        string newPath = ModManager.DisableMod(this);
                        IsEnabled = false;
                        FullFolderPath = newPath;
                        IconPath = ModScanner.FindModImagePath(FullFolderPath);
                    }
                    catch (IOException)
                    {
                        throw new ModLockedException("This mod cannot be disabled, as a file in it is being used by another program, or the folder already exists in the ");
                    }
                }
                else
                {
                    if (IsInOriginalInstallation())
                    {
                        throw new ModLockedException("This mod cannot be disabled, as it is part of the original installation of Cortex Command.");
                    }
                    if (IsCreatedSkirmishMod())
                    {
                        throw new ModLockedException("This mod cannot be disabled, as it it required for the Activities Manager.");
                    }
                }
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

        /// <summary>Deletes the mod from the filesystem.</summary>
        public void Delete()
        {
            ModManager.DeleteMod(this);
        }
    }
}
