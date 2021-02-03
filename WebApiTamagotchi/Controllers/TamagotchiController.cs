﻿using Microsoft.AspNetCore.Mvc;
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
                List<ActionOption> list = context.GetAllGames();
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
                ActionOption actionOption = new ActionOption
                {
                    OptioName = actionOptionDTO.OptioName,
                    OptionEffect = actionOptionDTO.OptionEffect,
                    OptionId = actionOptionDTO.OptionId
                };
                context.DoActionPlay(actionOption, pDto.PlayerId);
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            }
        }

        [Route("GetAllFood")]
        [HttpGet]
        public List<ActionOptionDTO> GetAllFood()
        {
            PlayerDTO pDto = HttpContext.Session.GetObject<PlayerDTO>("player");
            //Check if user logged in!
            if (pDto != null)
            {
                List<ActionOption> list = context.GetFoodList();
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


        [Route("DoActionFeed")]
        [HttpPost]
        public void DoActionFeed([FromBody] ActionOptionDTO actionOptionDTO)
        {
            PlayerDTO pDto = HttpContext.Session.GetObject<PlayerDTO>("player");
            //Check if user logged in!
            if (pDto != null)
            {
                ActionOption actionOption = new ActionOption
                {
                    OptioName = actionOptionDTO.OptioName,
                    OptionEffect = actionOptionDTO.OptionEffect,
                    OptionId = actionOptionDTO.OptionId
                };
                context.feed(actionOption, pDto.PlayerId);
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            }
        }

        [Route("Update")]
        [HttpPost]
        public void Update([FromBody] PlayerDTO p)
        {
            PlayerDTO currentPlayer = HttpContext.Session.GetObject<PlayerDTO>("player");
            if (currentPlayer == null)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return;
            }
            Player player = context.Login(currentPlayer.PlayerEmail, currentPlayer.PlayerPassword);
            p.PlayerName = player.PlayerName;
            p.PlayerLastName = player.PlayerLastName;
            p.PlayerUsername = player.PlayerUsername;
            p.PlayerPassword = player.PlayerPassword;
            p.PlayerEmail = player.PlayerEmail;
            p.PlayerBirthDay = player.PlayerBirthDay;

            context.SaveChanges();
            HttpContext.Session.SetObject("player", p);
            context.SaveChanges();

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

        [Route("SignUp")]
        [HttpPost]
        public void SignUp([FromBody] PlayerDTO player)
        {
            TamagotchiBL.Models.Player p1 = new Player()
            {
                PlayerName = player.PlayerName,
                PlayerLastName = player.PlayerLastName,
                PlayerBirthDay = player.PlayerBirthDay,
                PlayerEmail = player.PlayerEmail,
                PlayerGenedr = player.PlayerGender,
                PlayerUsername = player.PlayerUsername,
                PlayerPassword = player.PlayerPassword
            };

            Player p = context.Players.Where(pp => pp.PlayerEmail == player.PlayerEmail).FirstOrDefault();
            if (p == null)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                context.Players.Add(p1);
                context.SaveChanges();
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
            }
        }
        [Route("GetPets")]
        [HttpGet]
        public List<PetDTO> GetPets()
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
