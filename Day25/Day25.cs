using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using Xunit;

//while (true)
new Day25().Test_part1_example();

public class Day25
{

    public readonly record struct Node(int i)
    {
        public Node(string s) : this(toint(s)) { }

        static int toint(string s)
        {
            if (s.Length != 3)
                throw new NotImplementedException();
            return (s[0] << 16) | (s[1] << 8) | (s[2] << 0);
        }
    }
    public readonly record struct Graph(Dictionary<Node, (int size, Dictionary<Node, int> connections)> v);
    static void Dump(Graph graph)
    {
        foreach (var kv in graph.v)
        {
            Node src = kv.Key;
            Console.Write((src, kv.Value.size));
            Console.Write(':');
            foreach (var dst in kv.Value.connections)
            {
                Console.Write(' ');
                Console.Write(dst);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    static Graph Load(string path)
    {
        Graph graph = new(new());
        void Add(Node lhs, Node rhs)
        {
            //string plus =
            //    ",bbn,msb,jcx,gfl,jhf,hlq,zlz,rbs,dcs,dph,hmf,djd,nxs,ltn,slq,tsm,xpg,vdn,czr,szv,cvb,bjr,gkj,gmd,dzs,mvq,gcn,scd,vkp,vdp,zct,xfp,qbv,qrv,lrz,bdn,fqq,ttm,xsp,jst,fsl,vpf,lmd,klc,vbg,mnc,zdc,gpk,pqf,rgl,klz,lcs,hvx,gfq,vgt,sxs,zgt,vhd,dmd,zxr,fjm,mgj,fhn,kxv,fgc,hgr,rqq,qns,zvt,dff,jkh,mpn,lbx,vpv,jms,ngm,lsp,qcj,gkl,pdf,cnr,qjf,fbx,gfn,hrn,hnb,xnz,hjc,cvk,bts,hcl,rsp,tqb,hfb,mbz,htl,dbc,qhr,ztx,mzz,pqq,ndg,hhp,mnj,ztt,sqx,nhb,rxk,ghf,dvf,lcp,cvs,tnz,qpj,dbq,nlv,mns,fjz,dqn,psq,vzj,lbd,hcp,jdf,txb,qlc,mmk,jlc,tmj,qbr,fsx,rkb,gbq,rkl,rbn,bxb,mcx,hpc,hdm,tfq,nbf,fkq,mfn,qds,jfj,tkl,zqk,bfh,czm,mpg,djh,gmk,tsr,nlh,zcz,bqp,knz,pcc,ckx,xvf,zfz,xsn,lpd,mfm,rvb,ntn,fgt,tcc,rcf,tbv,vrl,sdj,pfd,gbc,hsr,jkm,kcc,nrd,zrc,mgm,ptb,jvl,hmh,qbz,kdx,dht,tbh,kmv,gjv,gjq,dxz,hvb,tfx,kpm,npk,bdx,bvc,pdl,ppz,sln,zkk,dxn,vrt,shl,jnt,kmm,jlh,tkf,tgb,dnc,zdh,trr,jct,jpf,bnp,lzv,npm,mst,kfk,hvr,jdc,tqv,tcj,cns,txg,gnc,jpp,rjm,dxr,dlc,vlk,qpd,klv,dqg,sfg,rgr,bbx,fdc,tsb,dsh,nlp,ggf,nhv,vvr,zkv,fgd,zqn,csx,jlb,djg,pgt,mlg,xzc,lkn,hst,pct,tzj,cmj,lgx,vxm,ltt,htm,ssl,tql,bfr,ctl,qfk,xbj,zpr,cmr,nhq,fml,cvp,zbp,jfs,mms,qsj,xfn,sql,ttk,llt,lhj,spb,kpj,flq,hmm,kts,mdg,vkn,dqm,kjl,phf,jsg,rvm,hsh,nbl,xmb,xkt,fhz,hpr,fcv,xzz,dnz,ltv,rqx,bzx,plc,cpr,krn,jzb,gmm,vkx,fjk,rtg,lvt,mch,vsf,vvd,jmq,pld,nkh,mnt,fzl,tcr,nbh,psk,hvz,tfp,qgz,jbg,mbr,pnx,fmg,gms,flm,gcd,xvn,nnl,pbh,zzx,ffd,clb,mrv,xxs,zzl,xzk,xnn,nrv,lcq,vfc,pmd,lvv,hcx,mfj,xfl,skx,gtt,vkz,hng,vdh,lmr,ddf,hhc,kpd,mjd,hrl,jbm,dzt,mrm,xcx,xhl,pgj,qfm,czj,mzs,kgd,fxq,kvg,xtp,brd,fxx,xdt,kpc,sgf,rml,ppn,jxg,smd,ssm,hmp,fmj,hdx,kkd,mcg,dlx,hxr,crx,pfc,mff,bgm,jgc,cfc,tsf,xcj,hvq,jfk,jts,xzv,bjp,crk,mhb,mxs,fxk,pcz,vhg,zgr,qxn,pnd,ggk,hcs,qjp,mgx,fdx,mqp,fjt,bzt,gsg,jrz,qxb,csj,grx,gzn,jnk,gfx,jbk,bcd,vgp,jsm,pfx,xtx,xhg,lvb,zjq,psh,bkc,dlb,mnr,kvr,rdz,rlf,xpf,dcr,rzc,kjs,brn,srl,fxz,lqx,vth,bpv,ppl,mzx,dmg,qlr,qnj,thv,sgp,qnm,zqm,qdt,fbk,kkq,mfk,hnk,smr,lxn,kpf,ctn,blt,xzb,cpp,ksd,tjj,qrm,fgv,nhl,vfd,hfc,fgq,gxk,xkz,ddl,jfq,kmh,gxg,dnm,jqs,jvh,ddm,dxh,mkk,cfr,nrn,fbf,pgh,chn,tss,znk,hks,lvk,sfd,pgb,dcp,lrd,zsm,kqd,rdx,kkn,pbc,qzv,qsm,jbh,rqr,pvq,bnx,nhf,qjt,kxg,rzg,lzr,mtq,fsh,lsx,mkh,kqx,qvf,bbt,jls,qmz,vxk,bjt,smj,scv,xxk,msx,nqr,xjt,nbn,xnx,krb,xcl,ghc,zhv,lsl,svx,jrq,fgp,zll,tqm,tlz,bjm,xvd,bzs,vzn,bpg,bcg,sbv,kml,lnp,ndv,kjb,lch,qff,rkc,jxj,pns,slt,gts,njq,mdp,khh,kbl,qdg,dhq,mrt,pxl,qbj,jtc,dpt,mmb,qbs,btk,szf,scf,dxf,ncq,pvh,gfb,fhp,hmd,hvm,tkr,clh,hpj,zxt,vdj,qxd,tpn,jkx,lss,jmg,rcc,hjg,mdm,mft,fxv,vqb,pkq,nkp,bkd,vgh,lkh,dbx,lvd,cnp,jfv,kdk,sft,scj,lcc,sfs,pbj,rvt,qxl,rsv,slv,nhx,rpj,jzn,qgr,rrr,hkf,qcl,fnp,qrc,lkj,hhb,tcb,nkm,rkv,xqn,mdv,hrb,fgs,brb,gch,pkb,qnh,xdj,prh,cnh,tvh,hsm,nks,drp,jdp,kdh,vhb,qkl,tsl,fxm,xfj,bmz,sss,vpl,mjz,cxg,skd,krq,vkl,dtg,dps,cbn,gdt,ntl,xfs,ngg,psg,fch,srd,stp,nct,kfg,sxk,lvq,rlc,drx,rkp,fss,plb,bff,mtx,jmx,klf,fbl,rkm,hgl,jfd,nzn,jlr,jpv,gqm,mbf,dcl,djj,jqv,tmh,pgk,ngz,tvf,xvh,bkm,czp,jnv,gtq,hsj,bzr,qqq,ztc,hdk,kfq,jpl,xvb,zmm,jpc,gfs,ntb,jmh,sqz,fmx,ccf,jxn,lcf,tcd,kvz,lfs,mpq,dmm,zqg,pnm,rrf,cgk,btp,tkm,ttz,zvc,rqp,hmj,mfh,qbm,a,b,c,d,e,f,g,h";

            //lhs = (plus.Contains("," + lhs + ",") ? "+" : "-") + lhs;
            //rhs = (plus.Contains("," + rhs + ",") ? "+" : "-") + rhs;

            if (!graph.v.TryGetValue(lhs, out var node))
            {
                node = new();
                graph.v.Add(lhs, (1, new() { { rhs, 1 } }));
            }
            else
            {
                node.connections.Add(rhs, 1);
            }
        }
        foreach (var line in File.ReadLines(path))
        {
            var s = line.Split(new[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
            Node lhs = new(s[0]);
            foreach (var rhs in s.Skip(1))
            {
                var n = new Node(rhs);
                Add(lhs, n);
                Add(n, lhs);
            }
        }
        return graph;
    }
    static void Merge(Graph graph, Node a, Node b)
    {
        Trace.Assert(a.i.CompareTo(b.i) < 0);
        graph.v.Remove(a, out var na);
        graph.v.Remove(b, out var nb);
        Node name = a;
        Dictionary<Node, int> connections =
            na.connections.Concat(nb.connections)
            .GroupBy(x => x.Key)
            .Select(x => (x.Key, x.Sum(y => y.Value)))
            .ToDictionary();

        foreach (var c in connections)
        {
            graph.v[c.Key].connections.Remove(a);
            graph.v[c.Key].connections.Remove(b);
            graph.v[c.Key].connections.Add(name, c.Value);
        }
        graph.v.Add(
            name,
            (
                na.size + nb.size,
                connections
            ));
    }
    static void Solve(Graph graph)
    {
        int target_flow = 3;
        while (true)
        {
            var q =
                graph.v
                .Select(x => (src: x.Key, size: x.Value.size, connections: x.Value.connections.Where(y => x.Key.i.CompareTo(y.Key.i) < 0)))
                .Where(x => x.connections.Any())
                .Select(x => (x.src, x.size, dst: x.connections.First().Key, capacity: x.connections.First().Value));
            if (!q.Any())
                break;
            (Node src, int size, Node dst, int capacity) = q.First();

            Dictionary<(Node, Node), int> flowing = new();
            int find(Node curr, int flow, HashSet<Node> visited)
            {
                if (curr == dst)
                {
                    if (flow != 1)
                        "".ToString();
                    return flow;
                }
                foreach (var (next, next_capacity) in graph.v[curr].connections)
                {
                    flowing.TryGetValue((curr, next), out int curr_next_flow);
                    flowing.TryGetValue((next, curr), out int next_curr_flow);
                    Trace.Assert(curr_next_flow == 0 || next_curr_flow == 0);
                    int residual = next_capacity - curr_next_flow + next_curr_flow;
                    if (residual == 0)
                        continue;
                    if (!visited.Add(next))
                        continue;

                    int found_flow = find(next, Math.Min(flow, residual), visited);
                    if (found_flow > 0)
                    {
                        if (next_curr_flow <= found_flow)
                        {
                            if (flowing.Remove((next, curr)))
                                "".ToString();
                            int curr_next_flow2 = curr_next_flow + found_flow - next_curr_flow;
                            if (curr_next_flow2 == 0)
                                flowing.Remove((curr, next));
                            else
                                flowing[(curr, next)] = curr_next_flow2;
                        }
                        else
                        {
                            int next_curr_flow2 = next_curr_flow - found_flow;
                            Trace.Assert(next_curr_flow2 > 0);
                            flowing[(next, curr)] = next_curr_flow2;
                        }

                        //Console.Write(next);
                        //Console.Write(' ');
                        return found_flow;
                    }

                    //visited.Remove(next);
                }
                return 0;

            }
            flowing.Add((src, dst), capacity);
            int flow = capacity;
            while (true)
            {
                int found2 = find(src, int.MaxValue, [src]);
                if (found2 == 0)
                    break;
                flow += found2;
                //Console.WriteLine();
            }
            Trace.Assert(target_flow <= flow);
            if (flow == target_flow)
            {
                Trace.Assert(graph.v[src].connections.Remove(dst));
                Trace.Assert(graph.v[dst].connections.Remove(src));
                target_flow -= capacity;
                Console.WriteLine(("split", src, dst, capacity));
            }
            else
            {
                Trace.Assert(graph.v[src].connections.Remove(dst));
                Trace.Assert(graph.v[dst].connections.Remove(src));
                Console.WriteLine(("merge", src, dst));
                Merge(graph, src, dst);
            }
            //Console.WriteLine(("flow", src, dst, flow));
        }
    }

    [Fact]
    public void Test_part1_example()
    {
        var g = Load("example.txt");
        Solve(g);
        Assert.Equal(54, g.v.First().Value.size * g.v.Last().Value.size);
    }

    [Fact]
    public void Test_part1_input()
    {
        var g = Load("input.txt");
        Solve(g);
        Assert.Equal(598120, g.v.First().Value.size * g.v.Last().Value.size);
    }
}
