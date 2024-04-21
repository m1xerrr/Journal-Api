using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Journal.Domain.Responses;
using Journal.Domain.ResponseModels;

namespace Journal.Service.Interfaces
{
    public interface INoteService
    {
        Task<BaseResponse<NoteResponseModel>> CreateNote(string text, List<string> symbols, Guid userId);

        Task<BaseResponse<bool>> DeleteNote(Guid Id);

        Task<BaseResponse<NoteResponseModel>> EditNote(string text, List<string> symbols, Guid Id);

        Task<BaseResponse<List<NoteResponseModel>>> GetUserNotes(Guid userId);

        Task<BaseResponse<NoteResponseModel>> GetNoteById(Guid id);
    }
}
