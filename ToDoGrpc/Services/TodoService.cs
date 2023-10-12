using Grpc.Core;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

namespace ToDoGrpc;

public class TodoService : TodoIt.TodoItBase
{
    private readonly ApplicationDbContext _context;
    public TodoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public override async Task<CreateTodoResponse> CreateTodo(CreateToDoRequest request, ServerCallContext context)
    {
        if (request.Title == string.Empty || request.Description == string.Empty)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must suppy a valid object"));

        var todoItem = new ToDoItem
        {
            Title = request.Title,
            Description = request.Description,
        };

        await _context.ToDoItems.AddAsync(todoItem);
        await _context.SaveChangesAsync();

        return await Task.FromResult(new CreateTodoResponse
        {
            Id = todoItem.Id
        });
    }


    public override async Task<ReadTodoResponse> ReadTodo(ReadTodoRequest request, ServerCallContext context)
    {
        if (request.Id == 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must provide valid id"));
        }

        var todo = await _context.ToDoItems.FirstOrDefaultAsync(x => x.Id == request.Id);


        if (todo != null)
        {
            return await Task.FromResult(new ReadTodoResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                ToDoStatus = todo.ToDoStatus
            });
        }

        throw new RpcException(new Status(StatusCode.NotFound, "Data Not Found"));
    }
    public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
    {
        var response = new GetAllResponse();
        var itemList = await _context.ToDoItems.ToListAsync();

        foreach (var item in itemList)
        {
            response.ToDo.Add(new ReadTodoResponse
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                ToDoStatus = item.ToDoStatus
            });
        }

        return await Task.FromResult(response);
    }

    public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
    {

        if (request.Id <= 0) throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Argument"));

        var item = await _context.ToDoItems.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (item != null)
        {
            item.Description = request.Description;
            item.Title = request.Title;
            item.ToDoStatus = request.ToDoStatus;

            await _context.SaveChangesAsync();
            return await Task.FromResult(new UpdateToDoResponse
            {
                Id = item.Id
            });
        }

        throw new RpcException(new Status(StatusCode.NotFound, "Data Not Found"));
    }

    public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0) throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Argument"));

        var item = await _context.ToDoItems.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (item != null)
        {

            var res =  _context.ToDoItems.Remove(item);
            if(res != null) 
            {
               await _context.SaveChangesAsync();
            }

            return await Task.FromResult(new DeleteToDoResponse
            {
                Id = item.Id
            });
        }

        throw new RpcException(new Status(StatusCode.NotFound, "Data Not Found"));
    }
}