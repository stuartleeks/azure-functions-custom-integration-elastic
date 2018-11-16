package ElasticFunctionDemoJava;

import com.microsoft.azure.functions.*;

/**
 * The mock for HttpResponseMessage, can be used in unit tests to verify if the
 * returned response by HTTP trigger function is correct or not.
 */
public class OutputBindingMock<T> implements OutputBinding<T> {
    private T value;

    @Override
    public T getValue() {
        return this.value;
    }

    @Override
    public void setValue(T value) {
        this.value = value;
    }
}
