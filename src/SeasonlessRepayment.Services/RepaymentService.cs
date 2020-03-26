using System.Collections.Generic;
using System.Threading.Tasks;
using SeasonlessRepayment.Data;
using SeasonlessRepayment.Data.Repository;
using SeasonlessRepayment.Domain;

namespace SeasonlessRepayment.Services
{
    public class RepaymentService : IRepaymentService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IBaseRepository<Repayment> _repaymentRepository;
        private readonly ICustomerSummaryRepository _customerSummaryRepository;
        private readonly IBaseRepository<RepaymentUpload> _repaymentUploadRepository;

        public RepaymentService(AppDbContext appDbContext, IBaseRepository<Repayment> repaymentRepository,
            ICustomerSummaryRepository customerSummaryRepository,
            IBaseRepository<RepaymentUpload> repaymentUploadRepository)
        {
            _appDbContext = appDbContext;
            _repaymentRepository = repaymentRepository;
            _customerSummaryRepository = customerSummaryRepository;
            _repaymentUploadRepository = repaymentUploadRepository;
        }

        public async Task<List<Repayment>> ProcessUploads(IEnumerable<RepaymentUpload> repaymentUploads)
        {
            var repayments = new List<Repayment>();
            foreach (var repaymentUpload in repaymentUploads)
            {
                repayments.AddRange(await ProcessUpload(repaymentUpload));
            }

            return repayments;
        }

        public async Task<List<Repayment>> ProcessUpload(RepaymentUpload repaymentUpload)
        {
            var repayments = new List<Repayment>();

            if (repaymentUpload.SeasonId.HasValue)
            {
                repayments.Add(await ProcessUploadWithSeasonId(repaymentUpload));
            }
            else
            {
                repayments.AddRange(await ProcessUploadWithoutSeasonId(repaymentUpload));
            }

            _repaymentRepository.Add(repayments.GetRange(1, repayments.Count - 1));
            _repaymentUploadRepository.Add(repaymentUpload);
            await _appDbContext.SaveChangesAsync();
            return repayments;
        }

        private async Task<List<Repayment>> ProcessUploadWithoutSeasonId(RepaymentUpload repaymentUpload)
        {
            var customerSummaries =
                await _customerSummaryRepository.GetOutstandingByCustomerId(repaymentUpload.CustomerId);

            var repayments = new List<Repayment>();

            if (customerSummaries.Count <= 0)
            {
                repayments.Add(await ProcessUploadToLastDebt(repaymentUpload));
                return repayments;
            }

            var linkedList = new LinkedList<CustomerSummary>(customerSummaries);

            var current = linkedList.First;
            var amount = repaymentUpload.Amount;
            int? parentId = null;
            while (current != null && amount > 0)
            {
                var cs = current.Value;
                var result = DoAdjustment(new AdjustmentInput(amount, cs.TotalCredit - cs.TotalRepaid,
                    cs.CustomerId, repaymentUpload.Date, cs.SeasonId, parentId));

                if (result.RemainingAmount <= 0 || current == linkedList.Last)
                {
                    cs.TotalRepaid += amount;
                    repayments.Add(result.Repayment);
                    _customerSummaryRepository.Update(cs);
                    break;
                }

                amount = result.RemainingAmount;
                if (!parentId.HasValue)
                {
                    await _appDbContext.SaveChangesAsync();
                    parentId = result.Repayment.Id;
                }

                repayments.Add(result.Repayment);
                if (amount > 0)
                {
                    cs.TotalRepaid = cs.TotalCredit;
                    _customerSummaryRepository.Update(cs);
                    result = DoAdjustment(new AdjustmentInput(-amount, null, cs.CustomerId, repaymentUpload.Date,
                        cs.SeasonId,
                        parentId));
                    repayments.Add(result.Repayment);
                }

                current = current.Next;
            }

            return repayments;
        }

        private async Task<Repayment> ProcessUploadToLastDebt(RepaymentUpload repaymentUpload)
        {
            var customerSummary =
                await _customerSummaryRepository.GetLastByCustomerId(repaymentUpload.CustomerId);
            customerSummary.TotalRepaid += repaymentUpload.Amount;
            _customerSummaryRepository.Update(customerSummary);
            var repayment = repaymentUpload.ToRepaymentWithSeasonId(customerSummary.SeasonId);
            _repaymentRepository.Add(repayment);
            _appDbContext.RepaymentUploads.Add(repaymentUpload);
            return repayment;
        }

        private AdjustmentOutput DoAdjustment(AdjustmentInput adjustmentInput)
        {
            var rem = adjustmentInput.Amount - adjustmentInput.OutstandingDebt;
            var adjustmentResult = new AdjustmentOutput
            {
                RemainingAmount = rem < 0 ? 0 : rem,
                Repayment = new Repayment
                {
                    Amount = adjustmentInput.Amount,
                    CustomerId = adjustmentInput.CustomerId,
                    SeasonId = adjustmentInput.SeasonId,
                    Date = adjustmentInput.Date,
                    ParentId = adjustmentInput.ParentId
                }
            };
            _repaymentRepository.Add(adjustmentResult.Repayment);
            return adjustmentResult;
        }


        private async Task<Repayment> ProcessUploadWithSeasonId(RepaymentUpload repaymentUpload)
        {
            var customerSummary = await _customerSummaryRepository.GetByCustomerIdAndSeasonId(
                repaymentUpload.CustomerId,
                repaymentUpload.SeasonId.Value);

            customerSummary.TotalRepaid += repaymentUpload.Amount;
            _customerSummaryRepository.Update(customerSummary);
            var repayment = repaymentUpload.ToRepayment();
            _repaymentRepository.Add(repayment);
            _appDbContext.RepaymentUploads.Add(repaymentUpload);
            return repayment;
        }

        public Task<List<Repayment>> List()
        {
            return _repaymentRepository.List(r => r.Season.StartDate, r => r.Customer, r => r.Season);
        }
    }
}