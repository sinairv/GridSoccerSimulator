// Copyright (c) 2009 - 2011 
//  - Sina Iravanian <sina@sinairv.com>
//  - Sahar Araghi   <sahar_araghi@aut.ac.ir>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

package jsampleclient;

public class Program
{
    private static RandomClient client;

    public static void main(String[] args) throws Exception
    {
        try
        {
            if(args.length >= 2)
                client = new RandomClient("127.0.0.1", 5050, args[0], Integer.parseInt(args[1]));
            else
                client = new RandomClient("127.0.0.1", 5050, "JRandomClient", 1);
    
            client.Start();
        }
        catch (Exception e) 
        {
            System.err.println(e.getMessage());
            e.printStackTrace(System.err);
        }
    }
}
