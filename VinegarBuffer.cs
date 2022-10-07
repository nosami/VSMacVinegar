using System.Text;
using MonoDevelop.Core;

namespace Vinegar
{
    class VinegarBuffer
    {
        private readonly FilePath _filePath;

        private SortedList<string, VinegarOutput> _lines = new SortedList<string, VinegarOutput>();
        public VinegarBuffer(FilePath filePath)
        {
            _filePath = filePath;
        }

        public FilePath FilePath => _filePath;
        internal SortedList<string, VinegarOutput> Lines { get => _lines; set => _lines = value; }

        public string Build()
        {
            AddDirectory(_filePath);
            var sb = new StringBuilder();
            var location = new OriginalLocation(_filePath);
            sb.AppendLine($"{location}:");
            foreach(var output in _lines)
            {
                sb.AppendLine(output.Key);
            }
            return sb.ToString();
        }

        private void AddDirectory(FilePath filePath)
        {
            foreach (var dir in Directory.EnumerateDirectories(filePath))
            {
                var location = new DirectoryLocation(dir);
                _lines.Add(location.ToString(), location);
            }
            EnumerateFiles(filePath);
        }

        private void EnumerateFiles(string dir)
        {
            foreach (var file in Directory.EnumerateFiles(dir))
            {
                var location = new FileLocation(file);
                _lines.Add(location.ToString(), location);
            }
        }
    }

    abstract class VinegarOutput
    {
        public VinegarOutput(FilePath location)
        {
            Location = location;
        }

        public FilePath Location { get; }

        public override string ToString()
        {
            return Location.FileName;
        }
    }

    class OriginalLocation : VinegarOutput
    {
        public OriginalLocation(FilePath location) : base(location)
        {
        }

        public override string ToString()
        {
            return Location.FullPath;
        }
    }

    class FileLocation : VinegarOutput
    {
        public FileLocation(FilePath location) : base(location)
        {
        }
    }

    class DirectoryLocation : VinegarOutput
    {
        public DirectoryLocation(FilePath location) : base(location)
        {
        }
    }
}