using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IApiResponse
{
    bool IsSuccessful { get; }
    string Message { get; }
    object? Errors { get; }
}
