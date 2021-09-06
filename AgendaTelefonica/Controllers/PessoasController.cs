using AgendaTelefonica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgendaTelefonica.Controllers
{
    public class PessoasController : Controller
    {
        private readonly Contexto _contexto;

        public PessoasController(Contexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<IActionResult> Index(string Pesquisa = "")
        {
            //Ação para retornar a pesquisa na busca
            var q = _contexto.Pessoas.AsQueryable();
            if (!string.IsNullOrEmpty(Pesquisa))

                q = q.Where(c => c.Nome.Contains(Pesquisa));
            q = q.OrderBy(c => c.Nome.ToList());

            //Esse return já estava, não apagar
            return View(await _contexto.Pessoas.ToListAsync());
        }

        [HttpGet]

        public IActionResult CriarPessoa()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> CriarPessoa(Pessoa pessoa)
        {
            if (ModelState.IsValid == false)
            {
                //Verificação de CPF duplicado
                if (await _contexto.Pessoas.AnyAsync(x => x.CPF == pessoa.CPF))
                {
                    ModelState.Clear();
                    TempData["CPFDuplicado"] = "Esse CPF já está cadastrado";
                    return View();
                }
            }
            _contexto.Add(pessoa);
            await _contexto.SaveChangesAsync();
            TempData["ContatoNovo"] = $"Contato de {pessoa.Nome} {pessoa.Sobrenome} incluído com sucesso";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public async Task<IActionResult> AtualizarPessoa(int pessoaId)
        {
            Pessoa pessoa = await _contexto.Pessoas.FindAsync(pessoaId);
            if (pessoa == null)
                return NotFound();

            return View(pessoa);

        }

        [HttpPost]
        public async Task<IActionResult> AtualizarPessoa(Pessoa pessoa)
        {
            if (ModelState.IsValid)
            {
                //Verificação de CPF duplicado
                if (await _contexto.Pessoas.AnyAsync(x => x.CPF == pessoa.CPF))
                {
                    ModelState.Clear();
                    TempData["CPFDuplicado"] = "Esse CPF está sendo usado por outro usuário cadastrado";
                    //Manter a persistencia
                    return View();
                }
            }
            _contexto.Update(pessoa);
            await _contexto.SaveChangesAsync();
            TempData["ContatoAtualizado"] = $"Contato de {pessoa.Nome} {pessoa.Sobrenome} foi atualizado com sucesso";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<JsonResult> ExcluirPessoa(int pessoaId)
        {
            Pessoa pessoa = await _contexto.Pessoas.FindAsync(pessoaId);
            _contexto.Pessoas.Remove(pessoa);
            await _contexto.SaveChangesAsync();
            TempData["ContatoExcluido"] = $"Contato de {pessoa.Nome} {pessoa.Sobrenome} foi excluído com sucesso";
            return Json(true);

        }

    }
}
