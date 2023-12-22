using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Data.Repository;
// ReSharper disable InconsistentNaming

namespace Server.Controllers;

public class BaseController: ControllerBase
{
    protected readonly AppDbContext _dbContext;
    protected readonly IRepository _repository;

    public BaseController(AppDbContext dbContext, IRepository repository)
    {
        _dbContext = dbContext;
        _repository = repository;
    }
}