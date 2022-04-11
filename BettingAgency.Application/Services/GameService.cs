using System.Data;
using System.Data.SqlClient;
using BettingAgency.Application.Abstraction.IServices;
using BettingAgency.Application.Abstraction.Models;
using Microsoft.Data.Sqlite;

namespace BettingAgency.Application.Services;

public class GameService : IGameService
{
    private const int Factor = 9;

    private readonly SqliteConnection con = new(@"Data Source=Ubaid;Initial Catalog=test_db;Integrated Security=True");

    private int _accountBalance = 10000;

    public string PlaceBet(Request req)
    {
        var guessNumber = req.Number;
        var stake = req.Points;
        var prize = 0;
        var gewonnen = false;

        var random = new Random();
        var randomNumber = random.Next(0, Factor);

        if (guessNumber == randomNumber)
        {
            prize = stake * Factor;

            //add prize to the database
            _accountBalance += prize;
            gewonnen = true;
        }
        else
        {
            prize = stake * Factor;

            //add prize to the database
            _accountBalance -= prize;
            gewonnen = false;
        }


        var insertSQL =
            new SqliteCommand(
                "INSERT or replace INTO Book (token, balance) VALUES (?,?)",
                con);
        insertSQL.Parameters.Add(token);
        insertSQL.Parameters.Add(_accountBalance);
        try
        {
            insertSQL.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }


        var cmd = new SqlCommand("update", con);

        con.Open();
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("balance", _accountBalance);
        cmd.Parameters.AddWithValue("token", token);

        var i = cmd.ExecuteNonQuery();
        con.Close();

        var outpara = new SqlParameter("@token", SqlDbType.Text);
        outpara.ParameterName = "@token";
        outpara.SqlDbType = SqlDbType.Text;
        outpara.Direction = ParameterDirection.Output;
        cmd.Parameters.Add(outpara);
        cmd.Parameters.AddWithValue("@balance", _accountBalance);
        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();


        var winningStatement = "Congratulations, You won the bet";
        if (!gewonnen)
            winningStatement = "Unfortunately, you have lost the bet";

        return $"{winningStatement}. The number was :{randomNumber}. You new Balance is: {_accountBalance}";
    }
}