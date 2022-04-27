namespace DxAimArb
{
    public class CircuitHandlerService : CircuitHandler 
    {
        string circuitId = "";
        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            circuitId = circuit.Id;
            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }
        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            return base.OnCircuitClosedAsync(circuit, cancellationToken);
        }
    }
}
