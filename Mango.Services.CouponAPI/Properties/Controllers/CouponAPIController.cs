using AutoMapper;
using Azure.Core;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Properties.Controllers
{
    [Route("api/[Controller]")]
    public class CouponAPIController : Controller
    {
        private readonly AppDbContext _context;
        private ResponseDto _response;
        private readonly IMapper _mapper;

        public CouponAPIController(AppDbContext context,IMapper mapper)
        {
            _context = context;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        [Route("GetAllCoupons")]
        [HttpGet]
        public ResponseDto GetCoupons()
        {
            try
            {
                IEnumerable<Coupon> Coupons = _context.Coupons.ToList();
       
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(Coupons);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Route("GetCouponById/{id}")]
        [HttpGet]
        public ResponseDto GetCouponById(int id)
        {
            try
            {
                var Coupon = _context.Coupons.First(x => x.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(Coupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("GetByCod/{code}")]
        public ResponseDto GetCyCode(string code)
        {
            try
            {
                Coupon coupon = _context.Coupons.First(x => x.CouponCode == code);
                _response.Result = coupon;
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        public ResponseDto CreateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _context.Coupons.Add(coupon);
                _context.SaveChanges();
                _response.Result = couponDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        public ResponseDto UpdateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                //Coupon existingCoupon = _context.Coupons.First(x => x.CouponId == couponDto.CouponId);
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _context.Coupons.Update(coupon);
                _context.SaveChanges();
                _response.Result = couponDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        public ResponseDto DeleteCoupon(int id)
        {
            try
            {
                Coupon coupon = _context.Coupons.First(x => x.CouponId == id);
                _context.Coupons.Remove(coupon);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
