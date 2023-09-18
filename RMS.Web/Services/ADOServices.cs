using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using RMS.Web.Models.ViewModels;
using System.Data;

namespace RMS.Web.Services
{
    public class ADOServices
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MVCDBADO"].ConnectionString);
        

        public DataSet getdata()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_TrialbalanceHeirchy", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;
        }
        
        public DataSet GetAccGrpData()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Sp_Accounts", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;
        }
        public DataSet GetSubaccounts()
        {
            con.Open();

            SqlCommand cmd = new SqlCommand("Sp_AllAccountsHeiriachal", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;


        }
        public DataSet ShowAccountLedger()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("[Sp_AccLedger]",con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            con.Close();
            return ds;
        }

        public DataSet  Getmaterialcentresherichal()
        {
            con.Open();

            SqlCommand cmd = new SqlCommand("Sp_Allmaterialcentreherichal",con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;

        }

        public DataSet TrialBalance()
        {
            con.Open();

            SqlCommand cmd = new SqlCommand("Sp_TrialBalance", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;
        }
        
        public DataSet GetTrialBHierchal()
        {
            con.Open();

            SqlCommand cmd = new SqlCommand("[Sp_Trialbalancewithdate]", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;


        }

        public DataSet Getitemstree()
        {
            con.Open();

            SqlCommand cmd = new SqlCommand("Sp_AllItemsHeirchal", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;

        }


    }
}