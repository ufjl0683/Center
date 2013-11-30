import java.net.*;
import java.nio.charset.Charset;
import java.io.*;
public class jclient {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// TODO Auto-generated method stub
      java.net.Socket socket;
      try{
    	 // System.out.println("¤¤¤å");
      socket=new java.net.Socket(args[0],8000);
     // socket.bind(null);
      java.io.BufferedReader rd=new BufferedReader(new InputStreamReader(socket.getInputStream(),Charset.forName("UTF-16")));
      while(true)
      {
    	  
    	 System.out.println( rd.readLine());
    	 // socket.
      }
      
      
      }catch(Exception ex)
      {
    	  System.out.println(ex.getMessage());
    	  System.exit(-1);
      }
     
     
    
      }
	}


