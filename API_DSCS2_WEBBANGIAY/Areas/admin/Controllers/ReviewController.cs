using API_DSCS2_WEBBANGIAY.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_DSCS2_WEBBANGIAY.Areas.admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ShoesEcommereContext _context;

        public ReviewController(ShoesEcommereContext context)
        {
            _context = context;
        }

        //[HttpPost]
        //public IActionResult AddProductRating(StarReviewDetail body)
        //{
        //    var Star = _context.ReviewStars.Find(body.StarReviewID);
        //    if (Star != null)
        //    {
        //        Star.total++;
        //        switch (rating)
        //        {
        //            case 1:
        //                Star.MotSao++;
        //                break;
        //            case 2:
        //                Star.HaiSao++;
        //                break;
        //            case 3:
        //                Star.BaSao++;
        //                break;
        //            case 4:
        //                Star.BonSao++;
        //                break;
        //            case 5:
        //                Star.NamSao++;
        //                break;
        //            default:
        //                return BadRequest("Invalid rating value.");
        //        }

        //        Star.Avr = (Star.MotSao + Star.HaiSao * 2 + Star.BaSao * 3 + Star.BonSao * 4 + Star.NamSao * 5) / (double)Star.total;
        //        StarReviewDetail rv = new StarReviewDetail();
        //        rv.Comment = comment;
        //        rv.TenNguoiBinhLuan = customerName;
        //        rv.StarReviewID = starID;
        //        rv.MaSanPham = MaSanPham;
        //        return Ok();
        //    }
        //    else
        //    {
        //        return BadRequest("Invalid rating value.");
        //    }
            

        //}
    }
    }
