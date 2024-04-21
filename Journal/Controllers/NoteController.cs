using Journal.Domain.JsonModels;
using Journal.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Journal.Controllers
{
    public class NoteController : Controller
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpPost("AddUserNote")]
        public async Task<IActionResult> AddUserNote([FromBody] NoteJsonModel note)
        {
            var response = await _noteService.CreateNote(note.Text, note.Symbols, note.Id);
            return Json(response);
        }

        [HttpPost("DeleteUserNote")]
        public async Task<IActionResult> DeleteUserNote([FromBody] Guid id)
        {
            var response = await _noteService.DeleteNote(id);
            return Json(response);
        }

        [HttpPost("EditUserNote")]
        public async Task<IActionResult> EditUserNote([FromBody] NoteJsonModel note)
        {
            var response = await _noteService.EditNote(note.Text, note.Symbols, note.Id);
            return Json(response);
        }

        [HttpPost("UserNotes")]
        public async Task<IActionResult> UserNotes([FromBody] Guid userId)
        {
            var response = await _noteService.GetUserNotes(userId);
            return Json(response);
        }

        [HttpPost("NoteById")]
        public async Task<IActionResult> NoteById([FromBody] Guid Id)
        {
            var response = await _noteService.GetNoteById(Id);
            return Json(response);
        }
    }
}
