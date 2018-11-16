package ElasticFunctionDemoJava;

import java.util.*;
import com.microsoft.azure.functions.annotation.*;
import com.microsoft.azure.functions.*;

public class Wibble {
    String name;

    public void setName(String name)
    {
        this.name = name;
    }
    public String getName(){
        return this.name;
    }
}

