Imports System.IO
Imports System.Data.OleDb

Public Class Cliente_alta_New
    Dim DAcliente As New Datos.Cliente
    Dim DActacte As New Datos.CuentaCorriente
    Public cliente_id As Integer 'este parametro me lo envian de otro form
    Public form_procedencia As String = "alta"
    Public apertura As String = "menu_alta" 'esta variable me sirve para saber desde donde lo abro, si lo abro a la "Alta" desde el menu...el boton cancelar...solo borra lo q se escribe en los textbox.
    Private Sub Cliente_alta_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Show()
        tx_Fan.Focus()
        tx_Fan.TextAlign = HorizontalAlignment.Center
        tb_Dni_Cuit.TextAlign = HorizontalAlignment.Center
        ' tb_Dni_Cuit.MaxLength = 8
        tx_dir.TextAlign = HorizontalAlignment.Center
        tx_tel.TextAlign = HorizontalAlignment.Center
        'Habilitar()
        Obtener_provincias()
        Obtener_Iva()
        If form_procedencia = "modificar" Then
            recuperar_datos_cliente()
            tb_Dni_Cuit.ReadOnly = True
        End If
    End Sub


    Private Sub recuperar_datos_cliente()
        Dim ds_clie_recu As DataSet = DAcliente.Cliente_obtener_info(cliente_id)
        If ds_clie_recu.Tables(1).Rows.Count <> 0 Then
            tx_Fan.Text = ds_clie_recu.Tables(1).Rows(0).Item("CLI_Fan")
            tb_Dni_Cuit.Text = ds_clie_recu.Tables(1).Rows(0).Item("CLI_dni")
            Combo_Iva.SelectedValue = ds_clie_recu.Tables(1).Rows(0).Item("CLI_tipoiva")
            'tx_tel.Text = ds_clie_recu.Tables(1).Rows(0).Item("CLI_tel")
            'tx_dir.Text = ds_clie_recu.Tables(1).Rows(0).Item("CLI_dir")
            'tx_Cp.Text = ds_clie_recu.Tables(1).Rows(0).Item("CLI_CP")
            'combo_Prov.SelectedValue = ds_clie_recu.Tables(1).Rows(0).Item("CLI_Id_Prov")
            'Combo_Loc.SelectedValue = ds_clie_recu.Tables(1).Rows(0).Item("localidad_id")
            'tx_mail.Text = ds_clie_recu.Tables(1).Rows(0).Item("CLI_mail")


            'choco 17-12-2020 ahora cargo en el gridview las sucursales que tiene asignado el cliente
            Dim e As Integer = 0
            If ds_clie_recu.Tables(3).Rows.Count <> 0 Then
                While e < ds_clie_recu.Tables(3).Rows.Count
                    Dim fila As DataRow = Cliente_ds.Tables("Sucursales").NewRow
                    fila("SucxClie_id") = ds_clie_recu.Tables(3).Rows(e).Item("SucxClie_id")
                    fila("SucxClie_nombre") = ds_clie_recu.Tables(3).Rows(e).Item("SucxClie_nombre")
                    fila("SucxClie_Prov") = ds_clie_recu.Tables(3).Rows(e).Item("SucxClie_Prov")
                    fila("SucxClie_Loc") = ds_clie_recu.Tables(3).Rows(e).Item("SucxClie_Loc")
                    fila("EnBD") = "si"
                    fila("Provincia") = ds_clie_recu.Tables(3).Rows(e).Item("provincia")
                    fila("Localidad") = ds_clie_recu.Tables(3).Rows(e).Item("Localidad")
                    fila("SucxClie_tel") = ds_clie_recu.Tables(3).Rows(e).Item("SucxClie_tel")
                    fila("SucxClie_mail") = ds_clie_recu.Tables(3).Rows(e).Item("SucxClie_mail")
                    fila("SucxClie_dir") = ds_clie_recu.Tables(3).Rows(e).Item("SucxClie_dir")
                    fila("SucxClie_CP") = ds_clie_recu.Tables(3).Rows(e).Item("SucxClie_CP")
                    Cliente_ds.Tables("Sucursales").Rows.Add(fila)
                    e = e + 1
                End While
            End If

            'si tiene cuenta corriente cargamos los datos, solo se podra modificar el limite de deuda. choco: 02-12-2019.
            If ds_clie_recu.Tables(2).Rows.Count <> 0 Then
                CheckBox_habilitar_ctacte.Checked = True
                CheckBox_habilitar_ctacte.Enabled = False
                txt_ctacte.Text = ds_clie_recu.Tables(2).Rows(0).Item("CtaCte_id")
                txt_limitedeuda.Text = CDec(ds_clie_recu.Tables(2).Rows(0).Item("CtaCte_limitedeuda"))
                CheckBox_estado.Visible = True
                If CStr(ds_clie_recu.Tables(2).Rows(0).Item("CtaCte_estado")) = "Activo" Then
                    CheckBox_estado.Checked = True
                    CheckBox_estado.ForeColor = Color.Green
                    CheckBox_estado.Text = "Activo"
                Else
                    CheckBox_estado.Checked = False
                    CheckBox_estado.ForeColor = Color.Red
                    CheckBox_estado.Text = "Inactivo"
                End If
                Label_fechaalta.Text = "Fecha de alta: " + CDate(ds_clie_recu.Tables(2).Rows(0).Item("CtaCte_fechaalta"))
                Label_fechaalta.Visible = True
            End If
        End If

    End Sub



    Dim ds_iva As DataSet
    Public Sub Obtener_Iva()
        ds_iva = DAcliente.Obterner_Iva()
        Combo_Iva.DataSource = ds_iva.Tables(0)
        Combo_Iva.DisplayMember = "IVA_Descripcion"
        Combo_Iva.ValueMember = "IVA_id"

    End Sub
    Dim Turno As Integer = 0

    Public Sub Turno_Cliente(ByVal B As Integer)
        Turno = B
    End Sub

    Private Sub Bo_guardar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Dim result As DialogResult
        'result = MessageBox.Show("¿Desea dar de alta al Cliente?", "Sistema de Gestion.", MessageBoxButtons.OKCancel)
        'If result = DialogResult.OK Then
        '    'DAcliente.Cliente_Alta(tx_ape_clie.Text, tx_nom_clie.Text, tx_dni_clie.Text, DT_fecha.Value, tx_tel_clie.Text, tx_mail_clie.Text, tx_obser.Text, ComboBox1.Text)

        '    Select Case (Turno)
        '        Case (0)
        '            limpiar_deshabilitar()
        '            MessageBox.Show("El Cliente fue dado de alta correctamente.", "Sistema de Gestion.")
        '        Case (1)

        '            'If tb_Dni.Text = Nothing Then
        '            'If tx_ape_clie.Text <> Nothing Then
        '            '    TurnoFijo_alta.TX_ape.Text = tx_ape_clie.Text + " " + tx_nom_clie.Text
        '            'Else
        '            '    TurnoFijo_alta.TX_ape.Text = tx_nom_clie.Text
        '            'End If
        '            TurnoFijo_alta.Cliente_ObtenerNombre()
        '            Else
        '            TurnoFijo_alta.TX_doc.Text = tb_Dni.Text
        '            TurnoFijo_alta.Cliente_ObtenerDNI()
        '            End If
        'Me.Close()

        '        Case (2)

        'If tb_Dni.Text = Nothing Then
        '    'If tx_ape_clie.Text <> Nothing Then
        '    '    Turno_alta.TX_ape.Text = tx_ape_clie.Text + " " + tx_nom_clie.Text
        '    'Else
        '    '    Turno_alta.TX_ape.Text = tx_nom_clie.Text
        '    'End If

        '    Turno_alta.Cliente_ObtenerNombre()
        'Else

        '    Turno_alta.TX_doc.Text = tb_Dni.Text
        '    Turno_alta.Cliente_ObtenerDNI()
        'End If
        'Me.Close()

        '    End Select

        'End If
    End Sub



    'Private Sub tx_dni_clie_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tx_dni_clie.TextChanged
    '    Verificar()
    'End Sub

    'Private Sub tx_ape_clie_TextChanged(sender As System.Object, e As System.EventArgs) Handles tx_ape_clie.TextChanged
    '    Verificar()
    'End Sub

    'Private Sub tx_nom_clie_TextChanged(sender As System.Object, e As System.EventArgs) Handles tx_nom_clie.TextChanged
    '    Verificar()
    'End Sub

    'Public Sub Verificar()
    '    Dim ds_CLI_dni As DataSet = DAcliente.Cliente_VerificarDni(tb_Dni_Cuit.Text)
    '    With ds_CLI_dni.Tables(0).Rows
    '        If .Count  0 Then
    '            tb_Dni_Cuit.ForeColor = Color.Black
    '            ERROR_dni.Visible = False

    '        Else
    '            ERROR_dni.Visible = True
    '            tx_dni_clie.ForeColor = Color.Red
    '        End If
    '    End With
    '    Habilitar()
    'End Sub

    'Public Sub Habilitar()
    '    If tx_dni_clie.ForeColor = Color.Black And tx_dni_clie.TextLength >= 7 And tx_ape_clie.Text <> Nothing And tx_nom_clie.Text <> Nothing Then
    '        Bo_guardar.Enabled = True
    '    Else
    '        Bo_guardar.Enabled = False
    '    End If
    'End Sub

    Private Sub Bo_cancelar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        limpiar_deshabilitar()
    End Sub

    Public Sub limpiar_deshabilitar()
        'choco 17-12-2020
        Cliente_ds.Tables("Sucursales").Rows.Clear()
        '////////////////////////////////////////////////

        tx_Fan.Text = Nothing
        tb_Dni_Cuit.Text = Nothing
        tx_tel.Text = Nothing
        tx_dir.Text = Nothing
        tx_Cp.Text = Nothing
        tx_mail.Text = Nothing
        tx_Fan.Focus()
        form_procedencia = "alta"
        error_razonsocial.Visible = False
        error_dni.Visible = False
        CheckBox_habilitar_ctacte.Checked = False
        CheckBox_habilitar_ctacte.Enabled = True

        Label_fechaalta.Visible = False
        CheckBox_estado.Visible = False
    End Sub

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click

    End Sub
    Public FormProc As String

    Private Sub btn_Aceptar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Aceptar.Click
        If form_procedencia = "alta" Then
            Alta()
        End If
        If form_procedencia = "modificar" Then
            Modificar()
        End If
    End Sub

    Private Sub Modificar()
        Dim result As DialogResult
        result = MessageBox.Show("¿Desea modificar el Cliente?", "Sistema de Gestión", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
        If result = DialogResult.OK Then
            If DG_Servicio.Rows.Count <> 0 Then



                'DAcliente.Cliente_Modificar(cliente_id, tx_Fan.Text, tb_Dni_Cuit.Text, Combo_Iva.SelectedValue, tx_tel.Text, tx_dir.Text, tx_Cp.Text, combo_Prov.SelectedValue, Combo_Loc.SelectedValue, tx_mail.Text)
                DAcliente.Cliente_Modificar(cliente_id, tx_Fan.Text, tb_Dni_Cuit.Text, Combo_Iva.SelectedValue)

                'ahora veo si hace falta modificar o agrear una sucursal de las que estan en el gridview
                Dim e As Integer = 0
                While e < Cliente_ds.Tables("Sucursales").Rows.Count
                    Dim EnBD As String = CStr(Cliente_ds.Tables("Sucursales").Rows(e).Item("EnBD")).ToUpper

                    Select Case EnBD
                        Case "NO"
                            'AQUI HAGO UN ALTA
                            DAcliente.Cliente_Sucursales_alta(cliente_id, Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_nombre"),
                            Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_tel"),
                            Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_mail"),
                            Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_dir"),
                            CInt(Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_CP")),
                            CInt(Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_Prov")),
                            CInt(Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_Loc")),
                             "ACTIVO")
                        Case "MODIFICAR EN BD"
                            'aqui hago un update en sql
                            DAcliente.Cliente_Sucursales_modificar(CInt(Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_id")),
                                                                   Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_nombre"),
                            Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_tel"),
                            Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_mail"),
                            Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_dir"),
                            CInt(Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_CP")),
                            CInt(Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_Prov")),
                            CInt(Cliente_ds.Tables("Sucursales").Rows(e).Item("SucxClie_Loc")), "ACTIVO")
                    End Select
                    e = e + 1
                End While

                'ahora veo si doy de alta una cta cte o bien modifico una existente ligada a un cliente.
                If CheckBox_habilitar_ctacte.Checked = True Then
                    Dim ds_ctacte As DataSet = DActacte.CtaCte_buscar_id(CInt(txt_ctacte.Text))
                    If ds_ctacte.Tables(0).Rows.Count = 0 Then
                        'no existe, doy de alta
                        If txt_limitedeuda.Text = "" Then
                            txt_limitedeuda.Text = "0"
                        End If
                        DActacte.CteCte_alta(cliente_id, Now, CDec(0), CDec(txt_limitedeuda.Text))
                    Else
                        'existe, entonces modifico.
                        If txt_limitedeuda.Text = "" Then
                            txt_limitedeuda.Text = "0"
                        End If
                        DActacte.CtaCte_modificar(CInt(txt_ctacte.Text), CheckBox_estado.Text, CDec(txt_limitedeuda.Text))
                    End If
                End If

                Venta_Caja_gestion.Obtener_Clientes()
                MessageBox.Show("El Cliente se modificó correctamente.", "Sistema de Gestión.", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Cliente_modificar.Obtener_Clientes()
                limpiar_deshabilitar()
                Me.Close()
                Cliente_modificar.Close()
                Cliente_modificar.Show()
            Else
                MessageBox.Show("Error, Debe ingresar al menos una Sucursal.", "Sistema de Gestión.", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If
            limpiar_deshabilitar()
    End Sub


    Private Sub alta()
        If tb_Dni_Cuit.Text <> "" And tx_Fan.Text <> "" Then
            If DG_Servicio.Rows.Count <> 0 Then
                Dim result As DialogResult
                result = MessageBox.Show("¿Desea dar de alta al Cliente?.", "Sistema de Gestión.", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                If result = DialogResult.OK Then


                    Dim ds_CLI_dni As DataSet = DAcliente.Cliente_VerificarDni(tb_Dni_Cuit.Text)
                    With ds_CLI_dni.Tables(0).Rows
                        If .Count = 0 Then

                            Dim ds_cliente_alta As DataSet = DAcliente.Cliente_Alta_new(tx_Fan.Text, tb_Dni_Cuit.Text, Combo_Iva.SelectedValue)
                            'Dim ds_cliente_alta As DataSet = DAcliente.Cliente_Alta_new(tx_Fan.Text, tb_Dni_Cuit.Text, Combo_Iva.SelectedValue, tx_tel.Text, tx_dir.Text, tx_Cp.Text, combo_Prov.SelectedValue, Combo_Loc.SelectedValue, tx_mail.Text)
                            'choco: 02-12-2019 ////////////cuenta corriente
                            If CheckBox_habilitar_ctacte.Checked = True Then
                                'creamos un registro en la tabla cuentacorriente
                                Dim CLI_id As Integer = CInt(ds_cliente_alta.Tables(0).Rows(0).Item("CLI_id"))
                                If txt_limitedeuda.Text = "" Then
                                    txt_limitedeuda.Text = "0"
                                End If
                                DActacte.CteCte_alta(CLI_id, Now, CDec(0), CDec(txt_limitedeuda.Text))
                            End If

                            '/////////////CHOCO 17-12-2020 GUARDO SUCURSALES EN TABLA AUXILIAR DE CLIENTES: CLIENTE_SUCURSALES SE LLAMA///////////////
                            Dim e As Integer = 0
                            While e < DG_Servicio.Rows.Count
                                'Dim EnBD As String = DG_Servicio.Rows(e).Cells("EnBDDataGridViewTextBoxColumn").Value.ToString
                                Dim CLI_id As Integer = CInt(ds_cliente_alta.Tables(0).Rows(0).Item("CLI_id"))
                                DAcliente.Cliente_Sucursales_alta(CLI_id, DG_Servicio.Rows(e).Cells("SucxClienombreDataGridViewTextBoxColumn").Value,
                                                                  DG_Servicio.Rows(e).Cells("SucxClietelDataGridViewTextBoxColumn").Value,
                                                                  DG_Servicio.Rows(e).Cells("SucxCliemailDataGridViewTextBoxColumn").Value,
                                                                  DG_Servicio.Rows(e).Cells("SucxCliedirDataGridViewTextBoxColumn").Value,
                                                                  CInt(DG_Servicio.Rows(e).Cells("SucxClieCPDataGridViewTextBoxColumn").Value),
                                                                  CInt(DG_Servicio.Rows(e).Cells("SucxClieProvDataGridViewTextBoxColumn").Value),
                                                                  CInt(DG_Servicio.Rows(e).Cells("SucxClieLocDataGridViewTextBoxColumn").Value), "ACTIVO")
                                e = e + 1
                            End While
                            '/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                            Venta_Caja_gestion.Obtener_Clientes() 'esto creo q debo comentar
                            Cliente_modificar.Obtener_Clientes() 'esto creo que debo comentar
                            MessageBox.Show("El Cliente fue dado de alta correctamente.", "Sistema de Gestión.", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            'aqui veo si cierro el form y abro modificar, o blanqueo todo y sigo agregando
                            If apertura = "menu_alta" Then
                                limpiar_deshabilitar()
                            End If
                            If apertura = "formulario modificar" Then
                                Me.Close()
                                Cliente_modificar.Close()
                                Cliente_modificar.Show()
                            End If
                        Else
                            MessageBox.Show("Error, el Cliente ya existe.", "Sistema de Gestión.", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            tb_Dni_Cuit.Focus()
                            tb_Dni_Cuit.SelectAll()
                        End If
                    End With
                End If
            Else
                MessageBox.Show("Error, Debe ingresar al menos una Sucursal.", "Sistema de Gestión.", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            MessageBox.Show("Error, Ingrese los campos obligatorios", "Sistema de Gestión.", MessageBoxButtons.OK, MessageBoxIcon.Error)
            error_razonsocial.Visible = True
            error_dni.Visible = True
        End If


        If FormProc = "Mesas_Asignar" Then
            Mesa_asignar.Obtener_Clientes()
        End If
    End Sub

    Private Sub btn_Cancelar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Cancelar.Click
        If apertura = "menu_alta" Then
            limpiar_deshabilitar()
        End If

        If apertura = "formulario modificar" Then
            Me.Close()
            Cliente_modificar.Close()
            Cliente_modificar.Show()
        End If
    End Sub


#Region "Validaciones"
    Dim validaciones As New Validaciones

    
    Private Sub tx_tel_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles tx_tel.KeyPress
        validaciones.Restricciones_textbox(e, "Celular", tx_tel)
    End Sub


    Private Sub tx_Cp_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles tx_Cp.KeyPress
        validaciones.Restricciones_textbox(e, "Entero", tx_Cp)
    End Sub

    Private Sub tb_Dni_Cuit_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles tb_Dni_Cuit.KeyPress
        validaciones.Restricciones_textbox(e, "DNI/CUIT", tb_Dni_Cuit)
    End Sub


#End Region
    Dim ds_provincias As DataSet
    Dim idprov As Integer
    Private Sub Obtener_provincias()
        ds_provincias = DAcliente.Provincias_ObtenerTodo()
        'Cargar Provincias en ComboBox_prov
        combo_Prov.DataSource = ds_provincias.Tables(0)
        combo_Prov.DisplayMember = "provincia"
        combo_Prov.ValueMember = "Prov_id"
        idprov = combo_Prov.SelectedValue
        Obtener_localidades_x_provincias()
    End Sub


    Private Sub Obtener_localidades_x_provincias()

        Dim ds_localidades As DataSet = DAcliente.Obtener_localidades_x_provincias(idprov)
        'Cargar Provincias en ComboBox_prov
        Combo_Loc.DataSource = ds_localidades.Tables(0)
        Combo_Loc.DisplayMember = "Localidad"
        Combo_Loc.ValueMember = "id"



    End Sub
    Private Sub combo_Prov_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles combo_Prov.SelectedIndexChanged
        If idprov <> 0 Then
            idprov = combo_Prov.SelectedValue
            Obtener_localidades_x_provincias()
        End If
    End Sub

    'Private Sub tb_Dni_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles tb_Dni.KeyPress
    '    validaciones.Restricciones_textbox(e, "DNI/CUIT", tb_Dni)

    'End Sub



    

    Private Sub tx_Fan_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tx_Fan.GotFocus
        tx_Fan.SelectAll()
        tx_Fan.BackColor = Color.FromArgb(255, 255, 192)
    End Sub

    Private Sub tx_Fan_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tx_Fan.LostFocus
        tx_Fan.BackColor = Color.White
    End Sub

    Private Sub tb_Dni_Cuit_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tb_Dni_Cuit.GotFocus
        tb_Dni_Cuit.SelectAll()
        tb_Dni_Cuit.BackColor = Color.FromArgb(255, 255, 192)
    End Sub
    Private Sub tb_Dni_Cuit_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tb_Dni_Cuit.LostFocus
        tb_Dni_Cuit.BackColor = Color.White
    End Sub

    Private Sub Combo_Iva_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Combo_Iva.GotFocus
        Combo_Iva.SelectAll()
        Combo_Iva.BackColor = Color.FromArgb(255, 255, 192)
    End Sub

    Private Sub Combo_Iva_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Combo_Iva.LostFocus
        Combo_Iva.BackColor = Color.White
    End Sub

    Private Sub tx_tel_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tx_tel.GotFocus
        tx_tel.SelectAll()
        tx_tel.BackColor = Color.FromArgb(255, 255, 192)
    End Sub

    Private Sub tx_tel_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tx_tel.LostFocus
        tx_tel.BackColor = Color.White
    End Sub

    Private Sub tx_dir_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tx_dir.GotFocus
        tx_dir.SelectAll()
        tx_dir.BackColor = Color.FromArgb(255, 255, 192)
    End Sub

    Private Sub tx_dir_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tx_dir.LostFocus
        tx_dir.BackColor = Color.White
    End Sub

    Private Sub tx_Cp_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tx_Cp.GotFocus
        tx_Cp.SelectAll()
        tx_Cp.BackColor = Color.FromArgb(255, 255, 192)
    End Sub

    

    Private Sub tx_Cp_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tx_Cp.LostFocus
        tx_Cp.BackColor = Color.White
    End Sub

    Private Sub combo_Prov_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles combo_Prov.GotFocus
        combo_Prov.SelectAll()
        combo_Prov.BackColor = Color.FromArgb(255, 255, 192)
    End Sub
    Private Sub combo_Prov_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles combo_Prov.LostFocus
        combo_Prov.BackColor = Color.White
    End Sub

    Private Sub Combo_Loc_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Combo_Loc.GotFocus
        Combo_Loc.SelectAll()
        Combo_Loc.BackColor = Color.FromArgb(255, 255, 192)
    End Sub

    Private Sub Combo_Loc_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Combo_Loc.LostFocus
        Combo_Loc.BackColor = Color.White
    End Sub

    Private Sub tx_mail_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tx_mail.GotFocus
        tx_mail.SelectAll()
        tx_mail.BackColor = Color.FromArgb(255, 255, 192)
    End Sub

    Private Sub tx_mail_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles tx_mail.LostFocus
        tx_mail.BackColor = Color.White
    End Sub

    Private Sub txt_ctacte_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt_ctacte.KeyPress
        e.Handled = True
    End Sub

    Private Sub CheckBox_habilitar_ctacte_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckBox_habilitar_ctacte.CheckedChanged
        If CheckBox_habilitar_ctacte.Checked = True Then
            'aqui busco en la bd el ultimo nro de ctacte y cargo el proximo nro.
            'o sea genero de manera incrementa.
            txt_ctacte.Enabled = True

            Dim ds_ctacte As DataSet = DActacte.CtaCte_obtenertodo()
            Dim nro_ctacta As Integer
            If ds_ctacte.Tables(0).Rows.Count <> 0 Then
                nro_ctacta = CInt(ds_ctacte.Tables(0).Rows(0).Item("ID")) + 1
            Else
                nro_ctacta = 1
            End If
            txt_ctacte.Text = nro_ctacta

            'tambien habilito el textbox para colocar el limite de credito permitido.
            txt_limitedeuda.Enabled = True
            txt_limitedeuda.Focus()
        Else
            txt_ctacte.Enabled = False

            'tambien habilito el textbox para colocar el limite de credito permitido.
            txt_limitedeuda.Enabled = False
            txt_limitedeuda.Text = "0"

        End If
    End Sub

    Private Sub txt_limitedeuda_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt_limitedeuda.KeyPress
        validaciones.Restricciones_textbox(e, "Decimal", txt_limitedeuda)
    End Sub

    Private Sub CheckBox_estado_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckBox_estado.CheckedChanged
        If CheckBox_estado.Checked = True Then
            CheckBox_estado.ForeColor = Color.Green
            CheckBox_estado.Text = "Activo"
        Else
            CheckBox_estado.ForeColor = Color.Red
            CheckBox_estado.Text = "Inactivo"
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Cliente_Sucursales.Close()
        Cliente_Sucursales.Show()

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        'para quitar tengo 2 opciones.
        '1) si la columna del gridview que se llama EnBD esta con "no", entonces solo quito del data set. no actualizo en bd.
        '3) si la columna EnBD dice "si" entonces si la quito de la bd, actualizo el registro en la tabla y la pongo SucxClie_estado = "eliminado", es decir q es un borrado lógico. para no perder las facturas q puedan estar ligadas a la base de datos

        'entonce recorro el gridview.
        Dim pregunta As String = "no" 'esta variable la uso para preg 1 sola vez si estoy seguro de eliminar los elementos seleccionados en la grilla.
        Dim valido_seleccion As String = "no"
        If DG_Servicio.Rows.Count <> 0 Then
            Dim i As Integer = 0
            While i < DG_Servicio.Rows.Count
                If DG_Servicio.Rows(i).Cells("Item").Value = True Then 'el value en true significa que esta checkeado para eliminar
                    If pregunta = "no" Then
                        valido_seleccion = "si" 'la uso solamente para indicar q si tengo algo seleccionado en el gridview
                        If MsgBox("¿Esta seguro que quiere eliminar la sucursal seleccionada?.", MsgBoxStyle.YesNo, "Sistema de Gestión.") = MsgBoxResult.Yes Then
                            pregunta = "si"
                        Else
                            'aqui corto el ciclo, ya que se cancelo la eliminacion
                            i = DG_Servicio.Rows.Count
                        End If
                    End If
                    If pregunta = "si" Then
                        'primero guardo el nro de item q contiene
                        Dim item As Decimal = DG_Servicio.CurrentRow.Index
                        'valido eso de las 2 opciones que detalle arriba
                        Dim estado As String = DG_Servicio.Rows(i).Cells("EnBDDataGridViewTextBoxColumn").Value
                        If estado = "no" Then
                            'no esta guardado en bd, asi que solo quito
                        Else
                            'si esta en bd, entonces actualizo en bd, el estado del campo SucxClie_estado = "eliminado"
                            DAcliente.Cliente_Sucursales_eliminar(CInt(DG_Servicio.Rows(i).Cells("SucxClieidDataGridViewTextBoxColumn").Value), "ELIMINADO")
                        End If
                        DG_Servicio.Rows.RemoveAt(i)
                        i = 0 'lo reinicio, x q al quitar un ite, se reordenan los indices
                    End If
                Else
                    i = i + 1
                End If

            End While

            If pregunta = "si" Then
                MessageBox.Show("Eliminación correcta, los datos fueron actualizados.", "Sistema de Gestión.", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            If valido_seleccion = "no" Then
                MessageBox.Show("Seleccione una sucursal para eliminar.", "Sistema de Gestión.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        Else
            MessageBox.Show("No hay sucursales asignadas al cliente.", "Sistema de Gestión.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub

    Private Sub DG_Servicio_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles DG_Servicio.Click
        'NOTA: LO QUE HAGO AQUI ES QUE SOLO SE PERMITA HACER UN CHECK EN UNA SOLA FILA
        If DG_Servicio.Rows.Count <> 0 Then
            'DataGridView2.Rows(i).Cells("Item").Value = True
            'If DataGridView2.CurrentRow.Cells("item").Value = True Then
            Dim i As Integer = 0
            While i < DG_Servicio.Rows.Count
                If DG_Servicio.Rows(i).Cells("item").Value = True Then
                    DG_Servicio.Rows(i).Cells("item").Value = False
                End If
                i = i + 1
            End While
            'ahora solo tildo el actual
            DG_Servicio.CurrentRow.Cells("item").Value = True
            'End If
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

        If DG_Servicio.Rows.Count <> 0 Then
            'aqui mando los parametros q sean necesarios
            Cliente_Sucursales.Close()
            Cliente_Sucursales.operacion = "modificar"
            Cliente_Sucursales.tx_nombre.Text = DG_Servicio.CurrentRow.Cells("SucxClienombreDataGridViewTextBoxColumn").Value
            Cliente_Sucursales.tx_tel.Text = DG_Servicio.CurrentRow.Cells("SucxClietelDataGridViewTextBoxColumn").Value
            Cliente_Sucursales.tx_mail.Text = DG_Servicio.CurrentRow.Cells("SucxCliemailDataGridViewTextBoxColumn").Value
            Cliente_Sucursales.tx_dir.Text = DG_Servicio.CurrentRow.Cells("SucxCliedirDataGridViewTextBoxColumn").Value
            Cliente_Sucursales.tx_Cp.Text = DG_Servicio.CurrentRow.Cells("SucxClieCPDataGridViewTextBoxColumn").Value
            Cliente_Sucursales.provincia_id = DG_Servicio.CurrentRow.Cells("SucxClieProvDataGridViewTextBoxColumn").Value
            Cliente_Sucursales.localidad_id = DG_Servicio.CurrentRow.Cells("SucxClieLocDataGridViewTextBoxColumn").Value
            Cliente_Sucursales.nombre_sucursal = DG_Servicio.CurrentRow.Cells("SucxClienombreDataGridViewTextBoxColumn").Value
            Cliente_Sucursales.Show()


        Else
            MessageBox.Show("Debe seleccionar una sucursal para modificar.", "Sistema de Gestión.", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub
End Class