package ElasticFunctionDemoJava;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

import com.microsoft.azure.functions.annotation.BlobInput;
import com.microsoft.azure.functions.annotation.BlobOutput;

@Target(ElementType.PARAMETER)
@Retention(RetentionPolicy.RUNTIME)
public @interface ElasticOutput {
    /**
     * Defines the trigger metadata name or binding name defined in function.json.
     * @return The trigger metadata name or binding name.
     */
	public BlobOutput superBlob(); //Note: This can be any type defined in azure-functions-java-library
	public String name(); // This is required
	public String index();
	public String indexType();
}

//Sample custom input binding
@Target(ElementType.PARAMETER)
	@Retention(RetentionPolicy.RUNTIME)
	@interface CustomInputBinding {
	    /**
	     * Defines the trigger metadata name or binding name defined in function.json.
	     * @return The trigger metadata name or binding name.
	     */
		BlobInput superBlob();
	    String name();
	}