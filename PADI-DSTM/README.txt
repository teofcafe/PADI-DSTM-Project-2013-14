Servidor: Quando uma instância é inicializada, é pedido o porto onde o servidor estará à escuta; Caso não seja introduzido um valor válido é usado um porto default (8082).

Master: Este deve ser inicilizado antes de cada outra instância. Não é necessário nenhum comando adicional.

Cliente: Quando o cliente é inicializado pede um command, que pode ser um dos seguintes: 
[begin] - Corresponde ao TxBegin();
[commit] - Corresponde ao TxCommit();
[abort] - Corresponde ao TxAbort();
[status] - Corresponde ao Status();
[fail] - Corresponde ao Fail(string URL) : o parâmetro de input é pedido posteriormente;
[freeze] - Corresponde ao Freeze(string URL) : o parâmetro de input é pedido posteriormente;
[recover] - Corresponde ao Recover(string URL) : o parâmetro de input é pedido posteriormente;
[create] - Corresponde ao CreatePadInt(int uid) : o parâmetro de input é pedido posteriormente;
[access] - Corresponde ao AccessPadInt(int uid) : o parâmetro de input é pedido posteriormente;
[read] - Corresponde ao Read() : é pedido o uid do objecto a ler, posteriormente;
[write] - Corresponde ao Write(int value) : é pedido o uid do objecto a escrever e após isso o valor a escrever;
