namespace Bookery.Node.Common;

public class PathBuilder
{
    private string _path = "";

    public string GetPath()
    {
        return _path;
    }

    public void Reset()
    {
        _path = "";
    }

    public void AddNode(string name)
    {
        if (_path == "")
        {
            _path += name;
        }
        else
        {
            _path += $"/{name}";
        }
    }

    public string GetTopNode()
    {
        string topNode;

        if (!_path.Contains("/"))
        {
            topNode = _path;
            _path = "";
        }
        else
        {
            topNode = _path.Substring(0, _path.IndexOf("/"));
            _path = _path.Substring(_path.IndexOf("/") + 1, _path.Length - _path.IndexOf("/") - 1);
        }

        return topNode;
    }

    public string GetLastNode()
    {
        string lastNode;

        if (!_path.Contains("/"))
        {
            lastNode = _path;
            _path = "";
        }
        else
        {
            lastNode = _path.Substring(_path.LastIndexOf("/") + 1, _path.Length - _path.LastIndexOf("/") - 1);
            _path = _path.Substring(0, _path.LastIndexOf("/"));
        }

        return lastNode;
    }

    public string GetLastNode(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return "";
        }
        path = path.TrimEnd('/');
        return path.Substring(path.LastIndexOf("/") + 1, path.Length - path.LastIndexOf("/") - 1);
    }

    public void ParseUri(Uri? uri)
    {
        _path = "";

        if (uri is null)
        {
            return;
        }

        if (uri.Segments.Length <= 1)
        {
            _path = "";
        }
        else
        {
            for (var i = 1; i < uri.Segments.Length; i++)
            {
                _path += uri.Segments[i];
            }
        }

        _path = _path.Replace("//", "/").TrimEnd('/');
    }

    public void ParsePath(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            path = "";
        }

        _path = path.TrimEnd('/');
    }

    public bool IsFile()
    {
        return _path.Contains(".");
    }

    public bool IsFile(string path)
    {
        return path.Contains(".");
    }

    public bool IsLastNode()
    {
        return !_path.Contains("/");
    }

    public int GetDepth(string path)
    {
        if (path.EndsWith("/"))
        {
            path = path.Substring(0, path.Length - 1);
        }

        return path.Count(x => x == '/') + 1;
    }
}