using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Domain.Exceptions.NotFound
{
    public class FileNotFound(string filePath) : NotFoundException_Base($"the file path {filePath} not found ")
    {
    }
}
