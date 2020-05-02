﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using emarket.Models;
using emarket.ViewModels;
using System.IO;
using System.Web.UI.WebControls;

namespace emarket.Controllers
{
    public class ProductsController : Controller
    {


        private ApplicationDbContext _context;

        public ProductsController()
        {
            _context=new ApplicationDbContext();
        }

        // GET: Products
        public ActionResult Index()
        {
            
            var products = _context.Products.ToList();
            
            return View("catalog-page",products);
        }




        public ActionResult New()
        {

            var viewModel = new ProductFormViewModel
            {
                
                Categories = _context.Categories.ToList()
            };

            return View("ProductForm2", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var productIndb= _context.Products.SingleOrDefault(p => p.Id == id);
            if (productIndb == null)
                return HttpNotFound();

            var viewModel = new ProductFormViewModel(productIndb)
            {
                Categories = _context.Categories.ToList()
            };

            return View("ProductForm2",viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Product product,ProductFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = _context.Categories.ToList();

                return View("ProductForm2", viewModel);
            }

            if (product.Id == 0)
            {
                product.Image = _set_product_image(product, viewModel.File);
                _context.Products.Add(product);
                _increase_number_of_products(product.CategoryId);
            }

            else
            {
                _SaveOfEdit(product,viewModel);
            }
           

            _context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }

        private void _SaveOfEdit(Product product,ProductFormViewModel viewModel)
        {
            //get selected product from database
            var productIndb = _context.Products.SingleOrDefault(p => p.Id == product.Id);
            //reset category number_of _products
            var categoryinDb = _context.Categories.SingleOrDefault(c => c.Id == productIndb.CategoryId);
            categoryinDb.Number_of_products--;

            //if image  updated
            if (viewModel.File != null)
            {

                var oldimgPath = Server.MapPath(productIndb.Image);
                System.IO.File.Delete(oldimgPath);
                productIndb.Image = _set_product_image(product, viewModel.File);
            }

            productIndb.Name = product.Name;
            productIndb.Price = product.Price;
            productIndb.description = product.description;
            productIndb.CategoryId = product.CategoryId;
            _increase_number_of_products(product.CategoryId);

        }






        private void _increase_number_of_products(int Categoryid)
        {
            //get the selected category from db
            var category = _context.Categories.SingleOrDefault(c => c.Id == Categoryid);
            //add 1 to number of products
            category.Number_of_products++;
        }

        private String _set_product_image(Product product, HttpPostedFileBase imgFile)
        {

            string path = "";
            //sst path to images folder
             path = "~/images/" + Path.GetFileName(imgFile.FileName);
            //save image on my server
            imgFile.SaveAs(Server.MapPath(path));
            //set the image path to product prop
            product.Image = path;
            return path;
        }

        public ActionResult Details(int id)
        {
            var productIndb = _context.Products.SingleOrDefault(p => p.Id == id);
            if (productIndb == null)
                return HttpNotFound();

            return View("product-page",productIndb);
        }

        public ActionResult Search(String search)
        {
            
            var products = _context.Products.Where(p => p.Category.Name == search).ToList();


            return View("catalog-page",products);
        }


        public ActionResult Delete(int id)
        {
            var product = _context.Products.SingleOrDefault(p => p.Id == id);

            //get the attached value from cart table to delete it fist because of foreign key
            var cartitem =_context.Cart.SingleOrDefault(c => c.Product_id == id);
            if(cartitem != null)
                _context.Cart.Remove(cartitem);
            if (product != null)
            {
                _context.Products.Remove(product);
            _context.SaveChanges();
            //delete image of product
            var oldimgPath = Server.MapPath(product.Image);
            System.IO.File.Delete(oldimgPath);

            }


            return RedirectToAction("Index");


        }



    }


    
}

