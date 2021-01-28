using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TamagotchiBL.Models;
using WebApiTamagotchi.DataTransferObjects;

namespace WebApiTamagotchi.Controllers
{
    [Route("api")]
    [ApiController]
    public class TamagotchiController : ControllerBase
    {
        #region Add connection to the db context using dependency injection
        TamagotchiContext context;
        public TamagotchiController(TamagotchiContext context)
        {
            this.context = context;
        }
        #endregion


        [Route("Test")]
        [HttpGet]
        public string Test()
        {
            return "Ofer is the king";
        }

        [Route("GetAllGames")]
        [HttpGet]
        public List<ActionOptionDTO> GetAllGames()
        {
            PlayerDTO pDto = HttpContext.Session.GetObject<PlayerDTO>("player");
            //Check if user logged in!
            if (pDto != null)
            {
                const int OPTION_PLAY = 3;
                List<ActionOption> list = context.ActionOptions.Where(a => a.ActionTypeId == OPTION_PLAY).ToList();
                List<ActionOptionDTO> listDTO = new List<ActionOptionDTO>();
                if (list != null)
                {
                    foreach (ActionOption a in list)
                    {
                        listDTO.Add(new ActionOptionDTO(a));
                    }
                }
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return listDTO;
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }
        }

        [Route("DoActionPlay")]
        [HttpPost]
        public void DoActionPlay([FromBody] ActionOptionDTO actionOptionDTO)
        {
            PlayerDTO pDto = HttpContext.Session.GetObject<PlayerDTO>("player");
            //Check if user logged in!
            if (pDto != null)
            {
                Player p = context.Players.Where(pp => pp.PlayerId == pDto.PlayerId).FirstOrDefault();
                List<Pet> pets = context.Pets.Where(ppp => ppp.PlayerId == p.PlayerId).ToList();
                const int DEAD_STATUS_ID = 4;
                Pet pet = pets.Where(p => p.LifeStatusId != DEAD_STATUS_ID).FirstOrDefault();
                pet.HappyLevel += (int)actionOptionDTO.OptionEffect;
                History h = new History()
                {
                    PetId = pet.PetId,
                    TimeOfAction = DateTime.Now,
                    OptionId = (int)actionOptionDTO.OptionId,
                    LifeCycleId = pet.LifeCycleId,
                    LifeStatusId = pet.LifeStatusId
                };
                context.AddHistory(h);
                context.SaveChanges();
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            }
        }

        [Route("Login")]
        [HttpGet]
        public PlayerDTO Login([FromQuery] string email, [FromQuery] string pass)
        {
            Player p = context.Login(email, pass);

            //Check user name and password
            if (p != null)
            {
                PlayerDTO pDTO = new PlayerDTO(p);

                HttpContext.Session.SetObject("player", pDTO);

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return pDTO;
            }
            else
            {

                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }
        }


        

        [Route("GetAnimals")]
        [HttpGet]
        public List<PetDTO> GetAnimals()
        {
            PlayerDTO pDto = HttpContext.Session.GetObject<PlayerDTO>("player");
            //Check if user logged in!
            if (pDto != null)
            {
                Player p = context.Players.Where(pp => pp.PlayerId == pDto.PlayerId).FirstOrDefault();
                List<PetDTO> list = new List<PetDTO>();
                if (p != null)
                {
                    foreach (Pet pet in p.Pets)
                    {
                        list.Add(new PetDTO(pet));
                    }
                }
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                return list;
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }
        }
    }
}
