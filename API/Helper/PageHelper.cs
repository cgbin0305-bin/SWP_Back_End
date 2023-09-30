namespace API.Helper;

public class PageHelper
{
    public static int CurrentPage(string pageString, int TotalElements, float pageResults)
    {
        int page;
        int currentPage;
        // each page site have 12 elements
        // total of elements in the list (which is have status = true)
        // total of page count
        var pageCount = Math.Ceiling(TotalElements / pageResults);
        try
        {
            // check string if empty => page = 0
            if (string.IsNullOrEmpty(pageString))
            {
                page = 0;
            }
            else
            {
                // try to parse string into int
                page = Int32.Parse(pageString);
            }
            currentPage = page;

        }
        catch (System.Exception)
        {
            // user enter double type => floor it 
            // Replace ',' to '.' Ex: instead of enter 1.2 but user enter 1,2 
            string input = pageString.Replace(',', '.');
            decimal outputDecimal;
            //  Parse string into decimal and floor it
            if (decimal.TryParse(input, out outputDecimal))
            {
                page = (int)Math.Floor(outputDecimal);
            }
            else
            {
                // if user enter string text => set page = 0
                page = 0;
            }
            // set current page
            currentPage = page;
        }
        // check if the page is out of range 0 || page Count 
        if (page < 0 || page > pageCount)
        {
            // return BadRequest("The result were not found");
            throw new Exception("The result were not found");
        }
        
        return currentPage;
    }
}