namespace MathExpressionParser;

public abstract class MathException(string message) : Exception(message);
public class MathParseException(string message) : MathException(message);
public class MathEvalException(string message) : MathException(message);