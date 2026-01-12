using CMCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMCS.Services
{
    public class InMemoryDataService
    {
        private List<Lecturer> _lecturers = new List<Lecturer>();
        private List<Contract> _contracts = new List<Contract>();
        private List<Claim> _claims = new List<Claim>();
        private List<SupportingDocument> _documents = new List<SupportingDocument>();
        private List<Approval> _approvals = new List<Approval>();

        private int _nextClaimId = 1;
        private int _nextDocumentId = 1;
        private int _nextApprovalId = 1;

        public InMemoryDataService()
        {
            SeedData();
        }

        private void SeedData()
        {
            // Seed Contracts
            _contracts.Add(new Contract { ContractId = 1, ContractName = "Standard Contract", HourlyRate = 350, StartDate = DateTime.Now.AddYears(-1) });
            _contracts.Add(new Contract { ContractId = 2, ContractName = "Senior Contract", HourlyRate = 500, StartDate = DateTime.Now.AddYears(-1) });

            // Seed Lecturers
            _lecturers.Add(new Lecturer { LecturerId = 1, FullName = "Dr. John Smith", Email = "john.smith@university.com", ContractId = 1 });
            _lecturers.Add(new Lecturer { LecturerId = 2, FullName = "Prof. Sarah Johnson", Email = "sarah.johnson@university.com", ContractId = 2 });
            _lecturers.Add(new Lecturer { LecturerId = 3, FullName = "Dr. Michael Brown", Email = "michael.brown@university.com", ContractId = 1 });
        }

        // Lecturer Methods
        public List<Lecturer> GetAllLecturers() => _lecturers;
        public Lecturer GetLecturerById(int id) => _lecturers.FirstOrDefault(l => l.LecturerId == id);
        public void AddLecturer(Lecturer lecturer)
        {
            lecturer.LecturerId = _lecturers.Any() ? _lecturers.Max(l => l.LecturerId) + 1 : 1;
            _lecturers.Add(lecturer);
        }
        public void UpdateLecturer(Lecturer lecturer)
        {
            var existing = GetLecturerById(lecturer.LecturerId);
            if (existing != null)
            {
                existing.FullName = lecturer.FullName;
                existing.Email = lecturer.Email;
                existing.ContractId = lecturer.ContractId;
            }
        }
        public void DeleteLecturer(int id)
        {
            var lecturer = GetLecturerById(id);
            if (lecturer != null) _lecturers.Remove(lecturer);
        }

        // Contract Methods
        public List<Contract> GetAllContracts() => _contracts;
        public Contract GetContractById(int id) => _contracts.FirstOrDefault(c => c.ContractId == id);

        // Claim Methods
        public List<Claim> GetAllClaims() => _claims;
        public Claim GetClaimById(int id) => _claims.FirstOrDefault(c => c.ClaimId == id);
        public List<Claim> GetClaimsByLecturer(int lecturerId) => _claims.Where(c => c.LecturerId == lecturerId).ToList();
        public List<Claim> GetClaimsByStatus(string status) => _claims.Where(c => c.Status == status).ToList();

        public Claim AddClaim(Claim claim)
        {
            claim.ClaimId = _nextClaimId++;
            claim.Status = "Submitted";
            claim.SubmittedAt = DateTime.Now;
            _claims.Add(claim);
            return claim;
        }

        public void UpdateClaimStatus(int claimId, string newStatus)
        {
            var claim = GetClaimById(claimId);
            if (claim != null)
            {
                claim.Status = newStatus;
            }
        }

        // Document Methods
        public List<SupportingDocument> GetDocumentsByClaim(int claimId) => _documents.Where(d => d.ClaimId == claimId).ToList();
        public SupportingDocument GetDocumentById(int id) => _documents.FirstOrDefault(d => d.DocumentId == id);

        public void AddDocument(SupportingDocument document)
        {
            document.DocumentId = _nextDocumentId++;
            document.UploadedAt = DateTime.Now;
            _documents.Add(document);
        }

        // Approval Methods
        public List<Approval> GetApprovalsByClaim(int claimId) => _approvals.Where(a => a.ClaimId == claimId).ToList();

        public void AddApproval(Approval approval)
        {
            approval.ApprovalId = _nextApprovalId++;
            approval.ApprovedAt = DateTime.Now;
            _approvals.Add(approval);
        }
    }
}