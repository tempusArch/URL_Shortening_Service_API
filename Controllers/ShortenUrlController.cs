using ShortenUrlApi.Models;
using ShortenUrlApi.Data;
using ShortenUrlApi.Mapper;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

namespace ShortenUrlApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ShortenUrlController : ControllerBase {
    private readonly ShortenUrlApiDbContext _context;
    private readonly IMapper _mapper;
    public ShortenUrlController(ShortenUrlApiDbContext context, IMapper mapper) {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<ShortUrlModelResponseDto>> ECreateShortUrlModel(string url) {
        if (string.IsNullOrEmpty(url))
            return BadRequest("Url is required");

        if(!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            return BadRequest("Invalid url format");

        if (_context.ShortUrlTable.Any(s => s.OriginalUrl == url))
            return BadRequest("Short code has been created for this url");

        var newOne = new ShortUrlModel {
            OriginalUrl = url,
            ShortCode = GenerateShortCode(6),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AccessCount = 0
        };

        _context.ShortUrlTable.Add(newOne);
        await _context.SaveChangesAsync();


        var result = _mapper.Map<ShortUrlModelResponseDto>(newOne);
        return Created(string.Empty, result);     
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<ShortUrlModelResponseDto>> EGetOriginalUrl(string code) {
        if (string.IsNullOrEmpty(code))
            return BadRequest("Short code is required");

        var existingOne = await _context.ShortUrlTable.SingleOrDefaultAsync(
            n => n.ShortCode == code
        );

        if (existingOne == null)
            return NotFound();

        existingOne.AccessCount++;
        await _context.SaveChangesAsync();

        var result = _mapper.Map<ShortUrlModelResponseDto>(existingOne);
        return Ok(result);
    }

    [HttpPut("{code}")]
    public async Task<ActionResult<ShortUrlModelResponseDto>> EUpdateShortUrlModel(string url, string code) {
        if (string.IsNullOrEmpty(url))
            return BadRequest("Url is required");

        if(!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            return BadRequest("Invalid url format");
        
        if (string.IsNullOrEmpty(code))
            return BadRequest("Short code is required");

        var existingOne = await _context.ShortUrlTable.SingleOrDefaultAsync(
            n => n.ShortCode == code
        );

        if (existingOne == null)
            return NotFound();

        existingOne.OriginalUrl = url;
        existingOne.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var result = _mapper.Map<ShortUrlModelResponseDto>(existingOne);
        return Ok(result);
    }


    [HttpDelete("{code}")]
    public async Task<ActionResult<string>> EDeleteShortUrlModel(string code) {
        if (string.IsNullOrEmpty(code))
            return BadRequest("Short code is required");

        var existingOne = await _context.ShortUrlTable.SingleOrDefaultAsync(
            n => n.ShortCode == code
        );

        if (existingOne == null)
            return NotFound();

        _context.ShortUrlTable.Remove(existingOne);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{code}/stats")]
    public async Task<ActionResult<ShortUrlModel>> EGetShortUrlModelWithStats(string code) {
        if (string.IsNullOrEmpty(code))
            return BadRequest("Short code is required");

        var existingOne = await _context.ShortUrlTable.SingleOrDefaultAsync(
            n => n.ShortCode == code
        );

        if (existingOne == null)
            return NotFound();

        return Ok(existingOne);
    }

    #region helper method
    private static string GenerateShortCode(int length = 6) {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[RandomNumberGenerator.GetInt32(chars.Length)])
            .ToArray());
    }

    #endregion
}