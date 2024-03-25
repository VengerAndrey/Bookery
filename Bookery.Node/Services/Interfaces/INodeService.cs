using Bookery.Node.Common.DTOs.Input;
using Bookery.Node.Common.DTOs.Output;

namespace Bookery.Node.Services.Interfaces;

public interface INodeService
{
    //Task<NodeDto?> Get(Guid id);
    //Task<bool> Delete(Guid id);
    Task<List<NodeDto>> GetByPath(string? path, Guid userId);
    Task<(NodeDto Node, string Path)> Create(string? path, CreateNodeDto createNodeDto, Guid userId);
    Task<(NodeDto Node, string Path)> Update(string? path, UpdateNodeDto updateNodeDto, Guid userId);
    Task Delete(string? path, Guid userId);
}