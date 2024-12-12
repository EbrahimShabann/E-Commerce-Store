using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using MailChimp.Net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using System.Text.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Services
{
    public class CartService
    {
        
            private readonly IHttpContextAccessor _httpContextAccessor;
       
            public CartService(IHttpContextAccessor httpContextAccessor )
            { 
                _httpContextAccessor = httpContextAccessor;
      
            }

     
            public void AddToCart(ShoppingCart cartItem)
            {
                var cart = GetCart();
                cart.Add(cartItem);
                SetCart(cart);
            }

          

            public void SetCart(List<ShoppingCart> cart)
            {
                _httpContextAccessor.HttpContext.Session.Set("Cart", cart);
            }

            
        public List<ShoppingCart> GetCart()
            {
            
                var cart = _httpContextAccessor.HttpContext.Session.Get<List<ShoppingCart>>("cart");            
                if (cart == null)
                {
               
                cart = new List<ShoppingCart>();
                }
                return cart;
            }

            

            public int GetCartCount()
            {
                var cart = GetCart();
                return cart.Count;
            }
        }
   
}
public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}