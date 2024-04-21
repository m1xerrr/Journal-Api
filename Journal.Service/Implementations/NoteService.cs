using Journal.Domain.ResponseModels;
using Journal.Domain.Responses;
using Journal.Service.Interfaces;
using Journal.Domain.Models;
using Journal.DAL.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace Journal.Service.Implementations
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<BaseResponse<NoteResponseModel>> CreateNote(string text, List<string> symbols, Guid userId)
        {
            var response = new BaseResponse<NoteResponseModel>();
            try
            {
                Note note = new Note();
                note.Id = Guid.NewGuid();
                note.Text = text;
                note.Symbols = symbols;
                note.UserId = userId;
                note.Time = DateTime.Now;

                if(await _noteRepository.Create(note))
                {
                    response.Data = new NoteResponseModel(note);
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Message = "Success";
                }
                else 
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "DB Error";
                }
            }
            catch (Exception ex) 
            {
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<bool>> DeleteNote(Guid Id)
        {
            var response = new BaseResponse<bool>();
            try
            {
                
                if(_noteRepository.SelectAll().FirstOrDefault(x => x.Id == Id) == null)
                {
                    response.StatusCode= Domain.Enums.StatusCode.ERROR;
                    response.Message = "Deal not found";
                    return response;
                }

                if (await _noteRepository.Delete(_noteRepository.SelectAll().FirstOrDefault(x => x.Id == Id)))
                {
                    response.Data = true;
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Message = "Success";
                }
                else
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "DB Error";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<NoteResponseModel>> EditNote(string text, List<string> symbols, Guid Id)
        {
            var response = new BaseResponse<NoteResponseModel>();
            try
            {
                Note note = _noteRepository.SelectAll().FirstOrDefault(x => x.Id == Id);
                if (note == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Deal not found";
                    return response;
                }

                note.Text = text;
                note.Symbols = symbols;

                if (await _noteRepository.Edit(note))
                {
                    response.Data = new NoteResponseModel(note);
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Message = "Success";
                }
                else
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "DB Error";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<NoteResponseModel>> GetNoteById(Guid id)
        {
            var response = new BaseResponse<NoteResponseModel>();
            try
            {
                Note note = _noteRepository.SelectAll().FirstOrDefault(x => x.Id == id);
                if (note == null)
                {
                    response.StatusCode = Domain.Enums.StatusCode.ERROR;
                    response.Message = "Deal not found";
                    
                }
                else
                {
                    response.StatusCode = Domain.Enums.StatusCode.OK;
                    response.Data = new NoteResponseModel(note);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<BaseResponse<List<NoteResponseModel>>> GetUserNotes(Guid userId)
        {
            var response = new BaseResponse<List<NoteResponseModel>>();
            try
            {
                var notes = _noteRepository.SelectAll().Where(x => x.UserId == userId);
                    
                var notesResponse = new List<NoteResponseModel>();
                foreach (var note in notes)
                {
                    notesResponse.Add(new NoteResponseModel(note));
                }
                response.StatusCode = Domain.Enums.StatusCode.OK;
                response.Data = notesResponse;
            }
            catch (Exception ex)
            {
                response.StatusCode = Domain.Enums.StatusCode.ERROR;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
