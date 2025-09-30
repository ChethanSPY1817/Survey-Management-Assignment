using System;

namespace SurveyManagement.Domain.Exceptions
{
    // Generic 404 Not Found
    public class NotFoundException : Exception
    {
        public NotFoundException(string entity, Guid id)
            : base($"{entity} with Id '{id}' was not found.") { }
    }
}
