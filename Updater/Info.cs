using Newtonsoft.Json;
using System.IO;
using System;

namespace Updater
{
    public class Info
    {
        private string fileName;

        public Info(string fileName)
        {
            this.fileName = fileName;
        }

        public string Name { get; set; }

        public string LocalFile { get; set; }
        public string LocalPath { get; set; }

        public string ReleaseFile { get; set; }
        public string ReleaseOwner { get; set; }
        public string ReleaseRepo { get; set; }

        public void Assign(Info source)
        {
            fileName = source.fileName;

            Name = source.Name;

            LocalFile = source.LocalFile;
            LocalPath = source.LocalPath;

            ReleaseFile = source.ReleaseFile;
            ReleaseOwner = source.ReleaseOwner;
            ReleaseRepo = source.ReleaseRepo;
        }

        public void Load()
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException();
            }

            Info info = null;

            using (var reader = File.OpenText(fileName))
            {
                info = JsonConvert.DeserializeObject<Info>(reader.ReadToEnd());
            }

            if (info == null ||
                string.IsNullOrWhiteSpace(info.Name) ||
                string.IsNullOrWhiteSpace(info.LocalFile) ||
                string.IsNullOrWhiteSpace(info.LocalPath) ||
                string.IsNullOrWhiteSpace(info.ReleaseFile) ||
                string.IsNullOrWhiteSpace(info.ReleaseOwner) ||
                string.IsNullOrWhiteSpace(info.ReleaseRepo)) throw new NullReferenceException();

            Assign(info);
        }
    }
}