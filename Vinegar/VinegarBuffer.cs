using System.Collections.Immutable;
using System.Text;
using MonoDevelop.Core;

namespace Vinegar
{
    public class VinegarBuffer
    {
        private readonly FilePath _filePath;

        private SortedList<string, VinegarOutput> _sortedLines = new SortedList<string, VinegarOutput>();
        public VinegarBuffer(FilePath filePath)
        {
            _filePath = filePath;
        }

        public FilePath FilePath => _filePath;
        SortedList<string, VinegarOutput> SortedLines { get => _sortedLines; set => _sortedLines = value; }
        public ImmutableList<VinegarOutput> Lines { get; private set; } = ImmutableList<VinegarOutput>.Empty;

        public string Build()
        {
            AddDirectory(_filePath);
            var location = new OriginalLocation(_filePath);
            Lines = new[] { location }.Union(SortedLines.Values).ToImmutableList();
            var sb = new StringBuilder();
            foreach(var output in Lines)
            {
                sb.AppendLine(output.ToString());
            }
            return sb.ToString();
        }

        private void AddDirectory(FilePath filePath)
        {
            foreach (var dir in Directory.EnumerateDirectories(filePath))
            {
                var location = new DirectoryLocation(dir);
                _sortedLines.Add(location.ToString(), location);
            }
            EnumerateFiles(filePath);
        }

        private void EnumerateFiles(string dir)
        {
            foreach (var file in Directory.EnumerateFiles(dir))
            {
                var location = new FileLocation(file);
                _sortedLines.Add(location.ToString(), location);
            }
        }
    }

    public abstract class VinegarOutput
    {
        public VinegarOutput(FilePath location)
        {
            Location = location;
        }

        public FilePath Location { get; }
        public bool Match { get; }
        public override string ToString()
        {
            return Location.FileName;
        }
    }

    public class OriginalLocation : VinegarOutput
    {
        public OriginalLocation(FilePath location) : base(location)
        {
        }

        public override string ToString()
        {
            return Location.FullPath + ":";
        }
    }

    public class FileLocation : VinegarOutput
    {
        public FileLocation(FilePath location) : base(location)
        {
        }
    }

    public class DirectoryLocation : VinegarOutput
    {
        public DirectoryLocation(FilePath location) : base(location)
        {
        }
    }
}