using LightApi.Domain.Entities;
using LightApi.EFCore.Repository;
using Microsoft.EntityFrameworkCore;

namespace LightApi.Service;

public class StudentService
{
    public IEfRepository<Student> Repository { get; set; }


  
}