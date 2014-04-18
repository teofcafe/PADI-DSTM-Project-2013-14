Servidor: Quando uma inst�ncia � inicializada, � pedido o porto onde o servidor estar� � escuta; Caso n�o seja introduzido um valor v�lido � usado um porto default (8082).

Master: Este deve ser inicilizado antes de cada outra inst�ncia. N�o � necess�rio nenhum comando adicional.

Cliente: Quando o cliente � inicializado pede um command, que pode ser um dos seguintes: 
[begin] - Corresponde ao TxBegin();
[commit] - Corresponde ao TxCommit();
[abort] - Corresponde ao TxAbort();
[status] - Corresponde ao Status();
[fail] - Corresponde ao Fail(string URL) : o par�metro de input � pedido posteriormente;
[freeze] - Corresponde ao Freeze(string URL) : o par�metro de input � pedido posteriormente;
[recover] - Corresponde ao Recover(string URL) : o par�metro de input � pedido posteriormente;
[create] - Corresponde ao CreatePadInt(int uid) : o par�metro de input � pedido posteriormente;
[access] - Corresponde ao AccessPadInt(int uid) : o par�metro de input � pedido posteriormente;
[read] - Corresponde ao Read() : � pedido o uid do objecto a ler, posteriormente;
[write] - Corresponde ao Write(int value) : � pedido o uid do objecto a escrever e ap�s isso o valor a escrever;
