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
        {   //Procura por nome
            var q = _contexto.Pessoas.AsQueryable();
            if (!string.IsNullOrEmpty(Pesquisa))

                q = q.Where(q => q.Nome.Contains(Pesquisa));

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

                //Verifica se a cidade é SP, e faz CPF ser Obrigatório 
                if (await _contexto.Pessoas.AnyAsync(a => a.Cidade == Cidade.SP) && pessoa.CPF == null)
                {
                    ModelState.Clear();
                    TempData["CPFObrigatorio"] = "O CPF é obrigatório";
                    return View();
                }

                //Verifica se é de MG e não deixa atualizar de for menor de idade
                if (await _contexto.Pessoas.AnyAsync(b => b.Cidade == Cidade.MG))
                {
                    int idade = DateTime.Now.Year - pessoa.DataNascimento.Year;
                    if (pessoa.DataNascimento.Date > DateTime.Now.Date)
                    {
                        idade--;
                    }
                    else if (idade < 18)
                    {
                        ModelState.Clear();
                        TempData["MenorIdade"] = "Você é menor de idade, não pode ser cadastrado";
                        return View();
                    }
                }
            }
            pessoa.DataHoraCadastro = DateTime.Now;
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
                //Verificação de CPF duplicado na atualização
                if (await _contexto.Pessoas.AnyAsync(x => x.CPF == pessoa.CPF))
                {
                    ModelState.Clear();
                    TempData["CPFDuplicado"] = "Esse CPF está sendo usado por outro usuário cadastrado";
                    return View(pessoa);

                }
                //Verifica se a cidade é SP, e faz CPF Obrigatório 
                if (await _contexto.Pessoas.AnyAsync(a => a.Cidade == Cidade.SP) && pessoa.CPF == null)
                {
                    ModelState.Clear();
                    TempData["CPFObrigatorio"] = "O CPF é obrigatório";
                    return View(pessoa);
                }
                //Verifica se é de MG e não deixa cadastrar de for menor de idade
                if (await _contexto.Pessoas.AnyAsync(b => b.Cidade == Cidade.MG))
                {
                    int idade = DateTime.Now.Year - pessoa.DataNascimento.Year;
                    if (idade < 18)
                    {
                        ModelState.Clear();
                        TempData["MenorIdade"] = "Você é menor de idade, não pode ser cadastrado";
                        return View(pessoa);
                    }
                }
            }
            pessoa.DataHoraCadastro = DateTime.Now;
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
