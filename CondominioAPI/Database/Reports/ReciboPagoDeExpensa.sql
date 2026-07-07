SET @startdate = NOW();
INSERT INTO REPORTS (Report_Name, Display_Name, Title_Style_Id, Display_Header, Display_Footer, Start_Date, End_Date)
VALUES ('ReciboPagoDeExpensa', 'Recibo de Pago de Expensa', 1, 1, 1, @startdate, NULL);


SET @reportId = LAST_INSERT_ID();

INSERT INTO report_headers (Report_Id, Display_Order, Style_Id, Display_Content, Is_Query, Start_Date, End_Date)
VALUES 
(@reportId, 0, 1, 'RECIBO PAGO DE EXPENSA', 1, @startDate, NULL),
(@reportId, 1, 2, ' SELECT \'Id Expensa\' as f1, \'Categoria\' as f2, \'Descripcion\' as f3, \'Monto\' as f4, \'Interes\' as f5, \'Fecha limite\' as f6, \'Departamento\' as f7, \'Monto Pagado\' as f8, \'Recibo\' as f9', 2, @startDate, NULL),
(@reportId, 2, 3, 'SELECT e.id AS f1, ec.Category AS f2, e.Description AS f3, e.Amount AS f4, e.Interest_Amount AS f5, e.Payment_Limit_Date AS f6, pr.Tower + \' \' + pr.Floor + \' \' + pr.Code AS f7, p.Amount AS f8, p.Receive_Number AS f9 FROM expenses e JOIN expense_payments ep ON e.id = ep.expense_id JOIN payments p ON ep.payment_id = p.id JOIN expense_categories ec ON e.Category_Id = ec.id JOIN property pr ON e.Property_Id = pr.id WHERE e.Id = @expenseId;', 1, @startDate, NULL),


INSERT INTO report_footers (Report_Id, Display_Order, Style_Id, Display_Content, Is_Query, Start_Date, End_Date)
VALUES 
(@reportId, 0, 1, 'Gracias por su pago.', 0, @startDate, NULL),
(@reportId, 1, 2, 'CONDOMINIO - AMBAR', 0, @startDate, NULL);


INSERT INTO report_params (Report_Id, Param_Name, Param_Type, Param_Description, Start_Date, End_Date)
VALUES 
(@reportId, 'expenseId', 'INT', 'Id de la expensa', @startDate, NULL);